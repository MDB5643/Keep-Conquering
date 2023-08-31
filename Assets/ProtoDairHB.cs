using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtoDairHB : MonoBehaviour
{
    private PolygonCollider2D pCollider;
    // Start is called before the first frame update
    void Start()
    {
        pCollider = transform.GetComponent<PolygonCollider2D>();
        LineRenderer lr = transform.GetComponent<LineRenderer>();
        if (lr == null)
        {
            lr = pCollider.gameObject.AddComponent<LineRenderer>();
        //
        }
        //
        ////2. Assign Material to the new Line Renderer
        ////lr.material = new Material(Shader.Find("Particles/Additive"));
        //
        //float zPos = 10f;//Since this is 2D. Make sure it is in the front
        //
        //if (pCollider is PolygonCollider2D)
        //{
        //    //3. Get the points from the PolygonCollider2D
        //    Vector2[] pColiderPos = (pCollider as PolygonCollider2D).points;
        //
        //    //Set color and width
        //    lr.SetColors(Color.green, Color.green);
        //    lr.SetWidth(.1f, .1f);
        //
        //    //4. Convert local to world points
        //    for (int i = 0; i < pColiderPos.Length; i++)
        //    {
        //        pColiderPos[i] = pCollider.transform.TransformPoint(pColiderPos[i]);
        //    }
        //
        //    //5. Set the SetVertexCount of the lr to the Length of the points
        //    lr.SetVertexCount(pColiderPos.Length + 1);
        //    for (int i = 0; i < pColiderPos.Length; i++)
        //    {
        //        //6. Draw the  line
        //        Vector3 finalLine = pColiderPos[i];
        //        finalLine.z = zPos;
        //        lr.SetPosition(i, finalLine);
        //
        //        //7. Check if this is the last loop. Now Close the Line drawn
        //        if (i == (pColiderPos.Length - 1))
        //        {
        //            finalLine = pColiderPos[0];
        //            finalLine.z = zPos;
        //            lr.SetPosition(pColiderPos.Length, finalLine);
        //        }
        //    }
        //}
        ////Since this is 2D. Make sure it is in the front
        ////Not Implemented. You can do this yourself
        //else if (pCollider is BoxCollider2D)
        //{
        //
        //}
       //zPos = 10f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
