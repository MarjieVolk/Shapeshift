using UnityEngine;
using System.Collections;

public class PlayerTransformer : MonoBehaviour {

    private GameObject currentTransformation;

    public delegate void PlayerTransformedHandler(GameObject target);
    public event PlayerTransformedHandler PlayerTransformed;

    public void TransformPlayer(GameObject target)
    {
        if(currentTransformation != null)
        {
            RevertPlayer();
        }

        if (PlayerTransformed != null) {
            PlayerTransformed(target);
        }
    }

    public void RevertPlayer()
    {
        if (PlayerTransformed != null)
        {
            PlayerTransformed(null);
        }
    }

	// Use this for initialization
	void Start () {
        PlayerTransformed += (target) =>
        {
            GetComponent<SpriteRenderer>().enabled = (target == null);
            GetComponent<Rigidbody2D>().isKinematic = (target != null);
        };
        PlayerTransformed += (target) =>
        {
            Destroy(currentTransformation);
            if (target != null)
            {
                currentTransformation = (GameObject)Instantiate(target, transform.position, Quaternion.identity);
                currentTransformation.transform.SetParent(transform);
            }
        };
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
