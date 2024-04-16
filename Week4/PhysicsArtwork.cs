using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsArtwork : MonoBehaviour
{
    public int numberOfObjects = 20;
    public float forceMagnitudeMin = 5f;
    public float forceMagnitudeMax = 15f;
    public GameObject sphere1Prefab;
    public GameObject sphere2Prefab;
    public GameObject lightPrefab;  
    public Vector3 windDirection = new Vector3(1, 0, 1);
    public float windStrength = 2f;

    private List<Rigidbody> rigidbodies = new List<Rigidbody>();

    void Start()
    {
        GameObject previousObject = null;

        for (int i = 0; i < numberOfObjects; i++)
        {
            GameObject obj = i % 2 == 0 ? Instantiate(sphere1Prefab) : Instantiate(sphere2Prefab);
            obj.transform.position = new Vector3(Random.Range(-10, 10), Random.Range(10, 20), Random.Range(-10, 10));

            Rigidbody rb = obj.AddComponent<Rigidbody>();
            rb.useGravity = true;
            rb.isKinematic = false;

            obj.AddComponent<SphereCollider>();

            obj.transform.rotation = Random.rotation;

            Vector3 forceDirection = Random.onUnitSphere;
            float forceMagnitude = Random.Range(forceMagnitudeMin, forceMagnitudeMax);
            rb.AddForce(forceDirection * forceMagnitude, ForceMode.Impulse);
            rb.AddTorque(Random.insideUnitSphere * forceMagnitude);

            if (previousObject != null && Random.value > 0.5f)
            {
                var jointType = Random.value > 0.5f ? typeof(SpringJoint) : typeof(HingeJoint);
                var joint = (Joint)obj.AddComponent(jointType);
                joint.connectedBody = previousObject.GetComponent<Rigidbody>();

                if (joint is SpringJoint sj)
                {
                    sj.spring = 50;
                    sj.damper = 1;
                    sj.autoConfigureConnectedAnchor = true;
                }
            }

            if (Random.value > 0.5f) 
            {
                GameObject lightObj = Instantiate(lightPrefab, obj.transform.position, Quaternion.identity, obj.transform);
                lightObj.transform.localPosition = Vector3.zero; 
            }

            previousObject = obj;
            rigidbodies.Add(rb);
        }

        StartCoroutine(ApplyWindForce());
    }

    IEnumerator ApplyWindForce()
    {
        while (true)
        {
            foreach (var rb in rigidbodies)
            {
                rb.AddForce(windDirection * windStrength);
            }
            yield return new WaitForSeconds(1); 
        }
    }
}

