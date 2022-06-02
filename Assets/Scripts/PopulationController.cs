using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class PopulationController : MonoBehaviour
{
    // population size
    public int pop_size = 100;
    // how many chromosome "chains"
    public int chrom_len = 80;
    // how many creatures will be removed each generation
    public float cutoff = 0.3f;
    // how many top performing creatures will be kept
    [Range(0.0f, 1.0f)]
    public float survivor_keep = 0.5f;
    // how many of surviving creatures will be mating
    [Range(0.0f, 1.0f)]
    public float will_mate = 0.33f;
    // how often will a child mutate
    [Range(0.0f, 1.0f)]
    public float mut_rate = 0.01f;
    // exploitation vs exploration
    [Range(0.0f, 1.0f)]
    public float alpha = 0.5f;
    // mutation deviation
    [Range(0.0f, 1.0f)]
    public float sigma = 0.05f;
    [Range(-10f, 0f)]
    public float min_bound = -3f;
    [Range(0f, 10f)]
    public float max_bound = 3f;
    public Transform start;
    public Transform end;
    
    public int converge_n;
    int converge_i;
    private float bestc = -1;

    // which creature will be used in the population
    public GameObject creature_prefab;
    List<Creature> population = new List<Creature>();

    public Text currGenText;
    public Text convergeText;
    public static int gen_num = 0;
    private Chromosome best_solution;

    private bool reset = false;

    public static void SetGenText(string s)
    {
        //currGenText.text = s;
    }

    private void Start()
    {
        if (!GameHandler.stopped)
            InitPopulation();
    }

    private void Update()
    {
        if (GameHandler.stopped == true)
        {
            ClearPop();
            reset = true;
        }
        else
        {
            if (reset)
            {
                reset = false;
                InitPopulation();
            }
            else
            {
                if (!HasActive())
                {
                    NextGeneration();
                }
            }

        }

    }

    private void ClearPop()
    {
        for (int i = 0; i < population.Count; i++) Destroy(population[i].gameObject);
        population.Clear();
    }

    private void InitPopulation()
    {
        for (int i = 0; i < pop_size; i++)
        {
            GameObject go = Instantiate(creature_prefab, start.position, Quaternion.identity);
            go.GetComponent<Creature>().InitCreature(new Chromosome(chrom_len, min_bound, max_bound), end.position);
            population.Add(go.GetComponent<Creature>());
        }
    }

    private void NextGeneration()
    {
        UpdateGenNum();
        int survivor_cut = Mathf.RoundToInt(pop_size * cutoff);
        List<Creature> survivors = new List<Creature>();

        // cutoff, discard the bad ones
        for (int i = 0; i < survivor_cut; i++)
        {
            Creature fittest = GetFittest();
            if (i == 0)
            {

                if (bestc == -1 || fittest.Fitness > bestc)
                {
                    
                    if (bestc != -1)
                    {
                        
                        if (fittest.Fitness > bestc + bestc * 0.05f)
                        {
                            converge_i = 0;
                            bestc = fittest.Fitness;
                            best_solution = fittest.chromosome;
                        }
                    }
                    else
                    {
                        bestc = fittest.Fitness;
                        best_solution = fittest.chromosome;
                    }
                    
                }
                 
            }
            survivors.Add(fittest);
        }

        // clear the population and its game objects
        for (int i = 0; i < population.Count; i++) Destroy(population[i].gameObject);
        population.Clear();

        // elitism, keep survivor_keep % of best performing survivors
        int elite_num = Mathf.RoundToInt(survivor_keep * survivors.Count);
        for (int i = 0; i < elite_num; i++)
        {
            GameObject go = Instantiate(creature_prefab, start.position, Quaternion.identity);
            go.GetComponent<Creature>().InitCreature(survivors[i].chromosome, end.position);
            population.Add(go.GetComponent<Creature>());
        }

        int mating_num = Mathf.RoundToInt(will_mate * survivors.Count);
        while (population.Count < pop_size)
        {
            for (int i = 0; i < survivors.Count; i++)
            {
                // selection
                Creature p1 = survivors[i];
                Creature p2 = survivors[Random.Range(0, mating_num)];

                // crossover
                List<Chromosome> kids = Crossover(p1.chromosome, p2.chromosome, alpha);
                Chromosome c1 = kids[0].Clone();
                Chromosome c2 = kids[1].Clone();

                // mutation
                float mut_chance = Random.Range(0.0f, 1.0f);
                if (mut_chance <= mut_rate) c1 = Mutate(c1);

                mut_chance = Random.Range(0.0f, 1.0f);
                if (mut_chance <= mut_rate) c2 = Mutate(c2);

                // keep in bounds
                ApplyBounds(c1);
                ApplyBounds(c2);

                // instantiate game objects for children,
                // initialize creatures that are tied to the game objects
                GameObject go1 = Instantiate(creature_prefab, start.position, Quaternion.identity);
                go1.GetComponent<Creature>().InitCreature(c1, end.position);

                population.Add(go1.GetComponent<Creature>());
                if (population.Count >= pop_size) break;

                GameObject go2 = Instantiate(creature_prefab, start.position, Quaternion.identity);
                go2.GetComponent<Creature>().InitCreature(c2, end.position);

                population.Add(go2.GetComponent<Creature>());
                if (population.Count >= pop_size) break;

            }
        }
        // clear the survivors and its game objects
        for (int i = 0; i < survivors.Count; i++) Destroy(survivors[i].gameObject);
        survivors.Clear();
        converge_i++;
        if(GameHandler.converged_continue == false)
        {
            if(converge_i == converge_n)
            {
                string best = "";
                for(int i = 0; i < best_solution.genes.Count; i++)
                {
                    best += "(" + best_solution.genes[i].x.ToString("F2") + ", " + best_solution.genes[i].y.ToString("F2") +  "), ";
                }
                GameHandler.bestSol = best;
                GameHandler.converged = true;

            }
        }
        

    }

    // BLX-a crossover
    private List<Chromosome> Crossover(Chromosome p1, Chromosome p2, float alpha)
    {
        Chromosome c1 = new Chromosome();
        Chromosome c2 = new Chromosome();

        for (int i = 0; i < p1.genes.Count; i++)
        {
            float d_x = Mathf.Abs(p1.genes[i].x - p2.genes[i].x);
            float d_y = Mathf.Abs(p1.genes[i].y - p2.genes[i].y);

            float min_x = Mathf.Min(p1.genes[i].x, p2.genes[i].x);
            float min_y = Mathf.Min(p1.genes[i].y, p2.genes[i].y);
            float max_x = Mathf.Max(p1.genes[i].x, p2.genes[i].x);
            float max_y = Mathf.Max(p1.genes[i].y, p2.genes[i].y);

            float x_val_1 = Random.Range(min_x - alpha * d_x, max_x + alpha * d_x);
            float x_val_2 = Random.Range(min_x - alpha * d_x, max_x + alpha * d_x);
            float y_val_1 = Random.Range(min_y - alpha * d_y, max_y + alpha * d_y);
            float y_val_2 = Random.Range(min_y - alpha * d_y, max_y + alpha * d_y);

            c1.genes.Add(new Vector2(x_val_1, y_val_1));
            c2.genes.Add(new Vector2(x_val_2, y_val_2));
        }

        return new List<Chromosome> { c1, c2 };
    }

    private Chromosome Mutate(Chromosome chromosome)
    {
        Chromosome res = chromosome.Clone();

        for (int i = 0; i < chromosome.genes.Count; i++)
        {
            float x = res.genes[i].x;
            float y = res.genes[i].y;

            float n_1 = (float)MathNet.Numerics.Distributions.Normal.Sample(0, sigma);
            float n_2 = (float)MathNet.Numerics.Distributions.Normal.Sample(0, sigma);

            res.genes[i] = new Vector2(x + n_1, y + n_2);

        }
        return res;
    }

    private void UpdateGenNum()
    {
        currGenText.text = "Gen : " + ++gen_num;
        convergeText.text = "Converge in : " + (converge_n - converge_i);
    }

    private Creature GetFittest()
    {
        float maxFitness = float.MinValue;
        int index = 0;
        for (int i = 0; i < population.Count; i++)
        {
            if (population[i].Fitness > maxFitness)
            {
                maxFitness = population[i].Fitness;
                index = i;
            }
        }
        Creature fittest = population[index];
        //so we dont get the same fittest every time
        population.Remove(fittest);
        return fittest;
    }

    private bool HasActive()
    {
        for (int i = 0; i < population.Count; i++)
        {
            if (!population[i].hasFinished)
            {
                return true;
            }
        }
        return false;
    }

    private void ApplyBounds(Chromosome dna)
    {
        for (int i = 0; i < dna.genes.Count; i++)
        {
            if (i < dna.genes.Count - 4)
            {
                if (dna.genes[i].x > max_bound)
                {
                    dna.genes[i] = new Vector2(max_bound, dna.genes[i].y);
                }
                if (dna.genes[i].y > max_bound)
                {
                    dna.genes[i] = new Vector2(dna.genes[i].x, max_bound);
                }
                if (dna.genes[i].x < min_bound)
                {
                    dna.genes[i] = new Vector2(min_bound, dna.genes[i].y);
                }
                if (dna.genes[i].y < min_bound)
                {
                    dna.genes[i] = new Vector2(dna.genes[i].x, min_bound);
                }
            }
            else
            {
                if (i + 1 == dna.genes.Count)
                {
                    if (dna.genes[i].x < 0.85f)
                    {
                        dna.genes[i] = new Vector2(0.85f, dna.genes[i].y);
                    }
                    if (dna.genes[i].x > 1.25f)
                    {
                        dna.genes[i] = new Vector2(dna.genes[i].x, 1.25f);
                    }
                }
            }


        }
    }

    public void SliderSpeedPopContr(float value)
    {
        if (!GameHandler.stopped)
            for (int i = 0; i < pop_size; i++)
            {
                population[i].creature_speed = value;
            }
    }

    public void SliderStepPopContr(float value)
    {
        if(!GameHandler.stopped)
            for (int i = 0; i < pop_size; i++)
            {
                population[i].path_multiplier = value;
            }
    }

    public void SliderPopSize(float value)
    {
        pop_size = Mathf.RoundToInt(value);
    }
    public void SliderChromosomeSize(float value)
    {
        chrom_len = Mathf.RoundToInt(value);
    }
    public void SliderCutoffSize(float value)
    {
        cutoff = value;
    }
    public void SliderKeepElite(float value)
    {
        survivor_keep = value;
    }
    public void SliderWillMate(float value)
    {
        will_mate = value;
    }
    public void SliderMutRate(float value)
    {
        mut_rate = value;
    }
    public void SliderAlpha(float value)
    {
        alpha = value;
    }
    public void SliderSigma(float value)
    {
        sigma = value;
    }
    public void SliderMinBound(float value)
    {
        if (!GameHandler.stopped)
            min_bound = value;
    }
    public void SliderMaxBound(float value)
    {
        if (!GameHandler.stopped)
            max_bound = value;
    }

}
