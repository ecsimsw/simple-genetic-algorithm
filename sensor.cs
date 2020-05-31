using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sensor : MonoBehaviour
{
    Car car;
    //testCar car;
    bool[] scoreLines;

    // Start is called before the first frame update
    void Start()
    {
        GameObject parent = transform.parent.gameObject;
        car = parent.GetComponent<Car>();
        //car = parent.GetComponent<testCar>();


    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "fence")
        {
            if (this.gameObject.name == "Left_Sensor")
            {
                car.sensor_detected |= 0b100;
            }

            if (this.gameObject.name == "Right_Sensor")
            {
                car.sensor_detected |= 0b001;
            }

            if (this.gameObject.name == "Middle_Sensor")
            {
                car.sensor_detected |= 0b010;
            }
        }
     }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "fence")
        {
            if (this.gameObject.name == "Left_Sensor")
            {
                car.sensor_detected |= 0b100;
            }

            if (this.gameObject.name == "Right_Sensor")
            {
                car.sensor_detected |= 0b001;
            }

            if (this.gameObject.name == "Middle_Sensor")
            {
                car.sensor_detected |= 0b010;
            }
            if (this.gameObject.name == "Body")
            {
                car.sensor_detected = 0b1111;
            }
        }
    }
}


// Sensor is only used by sensing. 
//just checking which sensor is on, and send that data to car  