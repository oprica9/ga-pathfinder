using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chromosome
{
    public List<Vector2> genes = new List<Vector2>();

    public Chromosome() { }

    public Chromosome(int chrom_len, float min_bound, float max_mound)
    {
        int i;
        // movement
        for (i = 0; i < chrom_len; i++)
        {
            genes.Add(new Vector2(Random.Range(min_bound, max_mound), Random.Range(min_bound, max_mound)));
        }
        // color
        for (int j = 0; j < 3; j++, i++)
        {
            genes.Add(new Vector2(Random.Range(0f, 1.0f), 0));
        }
        // size
        float r = Random.Range(0.85f, 1.25f);
        genes.Add(new Vector2(r, 0));

    }

    public Chromosome Clone() => new Chromosome
    {
        genes = this.genes
    };


}
