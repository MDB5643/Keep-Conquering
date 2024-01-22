using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtoDSpecHB : MonoBehaviour
{
    private PolygonCollider2D pCollider;
    private float lingerTime = 0.0f;
    public float maxLingerTime = 0.22f;
    // Start is called before the first frame update
    void Start()
    {
        
        pCollider = transform.GetComponent<PolygonCollider2D>();
        LineRenderer lr = transform.GetComponent<LineRenderer>();
        if (lr == null)
        {
            lr = pCollider.gameObject.AddComponent<LineRenderer>();

        }

        //2. Assign Material to the new Line Renderer
        //lr.material = new Material(Shader.Find("Particles/Additive"));

        float zPos = transform.parent.position.z;//Since this is 2D. Make sure it is in the front

        if (pCollider is PolygonCollider2D)
        {
            //3. Get the points from the PolygonCollider2D
            Vector2[] pColiderPos = (pCollider as PolygonCollider2D).points;

            //Set color and width
            lr.SetColors(Color.black, Color.black);
            lr.SetWidth(.3f, .3f);

            //4. Convert local to world points
            for (int i = 0; i < pColiderPos.Length; i++)
            {
                pColiderPos[i] = pCollider.transform.TransformPoint(pColiderPos[i]);
            }

            //5. Set the SetVertexCount of the lr to the Length of the points
            lr.SetVertexCount(pColiderPos.Length + 1);
            for (int i = 0; i < pColiderPos.Length; i++)
            {
                //6. Draw the  line
                Vector3 finalLine = pColiderPos[i];
                finalLine.z = zPos;
                lr.SetPosition(i, finalLine);

                //7. Check if this is the last loop. Now Close the Line drawn
                if (i == (pColiderPos.Length - 1))
                {
                    finalLine = pColiderPos[0];
                    finalLine.z = zPos;
                    lr.SetPosition(pColiderPos.Length, finalLine);
                }
            }
        }
        //Since this is 2D. Make sure it is in the front
        //Not Implemented. You can do this yourself
        else if (pCollider is BoxCollider2D)
        {
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        lingerTime += Time.deltaTime;
        if (lingerTime >= maxLingerTime && gameObject != null)
        {
            GameObject.Destroy(gameObject.transform.parent.gameObject);
        }
    }
}
