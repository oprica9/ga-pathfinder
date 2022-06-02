using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour
{
    public float creature_speed;
    public float path_multiplier;


    public Chromosome chromosome;

    //List<Vector2> path = new List<Vector2>();
    int path_index = 0;

    [HideInInspector]
    public bool hasFinished = false;
    bool has_been_initialized = false;
    bool hasCrashed = false;
    Vector2 target;
    Vector2 next_point;
    //LineRenderer line_renderer;

    SpriteRenderer sprite_renderer;

    public LayerMask obstacle_layer;

    public void InitCreature(Chromosome new_chromosome, Vector2 _target)
    {
        //path.Add(transform.position);
        //line_renderer = GetComponent<LineRenderer>();
        chromosome = new_chromosome;
        target = _target;
        next_point = transform.position;
        //path.Add(next_point);
        has_been_initialized = true;
        // size
        transform.localScale = new Vector2(chromosome.genes[chromosome.genes.Count - 1].x, chromosome.genes[chromosome.genes.Count - 1].x);
        sprite_renderer = GetComponent<SpriteRenderer>();
        // color
        sprite_renderer.color = new Color(
            chromosome.genes[chromosome.genes.Count - 2].x,
            chromosome.genes[chromosome.genes.Count - 3].x,
            chromosome.genes[chromosome.genes.Count - 4].x
        );
    }

    private void Update()
    {
        if (has_been_initialized && !hasFinished)
        {
            if (path_index == chromosome.genes.Count - 4 || Vector2.Distance(transform.position, target) < 0.5f)
            {
                hasFinished = true;
            }

            if ((Vector2)transform.position == next_point)
            {
                // every time we change next point
                next_point = (Vector2)transform.position + chromosome.genes[path_index] * path_multiplier;
                //path.Add(next_point);
                path_index++;
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, next_point, creature_speed * Time.deltaTime);
            }
        }
        // RenderLine();
    }

    //public void RenderLine()
    //{
    //    List<Vector3> linePoints = new List<Vector3>();

    //    if (path.Count > 2)
    //    {
    //        for (int i = 0; i < path.Count; i++)
    //        {
    //            linePoints.Add(path[i]);
    //        }
    //        linePoints.Add(transform.position);
    //    }
    //    else
    //    {
    //        linePoints.Add(path[0]);
    //        linePoints.Add(transform.position);
    //    }
    //    line_renderer.positionCount = linePoints.Count;
    //    line_renderer.SetPositions(linePoints.ToArray());
    //}

    public float Fitness
    {
        get
        {
            float dist = Vector2.Distance(transform.position, target);
            if (dist == 0)
            {
                dist = 0.00001f;
            }
            return (1 / dist) * (hasCrashed ? 0.2f : 1f);

        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 6)
        {
            hasCrashed = true;
            hasFinished = true;
        }
    }

    public void SliderSpeed(float value)
    {
        creature_speed = value;
    }

    public void SliderStep(float value)
    {
        path_multiplier = value;
    }

}
