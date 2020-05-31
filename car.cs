using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    [HideInInspector] public int value = 0xFFFFFF;/// x[2] | z[2] | sharp[8] | smooth[8] | forward[4]
    public int sensor_detected = 0b000;

    int sharpTurnValue;
    int smoothTurnValue;
    int backTurnValue;

    float speed = 5f;
    
    Rigidbody rb;
    Transform tr;

    public bool dead = true;
    bool halfLine = false;
    bool finish = false;
    public float time = 0f;
    public float record = 0f;
    bool[] scoreLines;

    int timeLimit = 15;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        tr = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!dead)
        {
            MoveCar();

            sensor_detected = 0b000;

            time += Time.deltaTime;
        }
        else
        {
            // Car is dead -> stop
        }
    }

    public void Initialize()
    {

        /// x[2] | z[2] | sharp[8] | smooth[8] | forward[4]

        int vX = ((value & 0xC00000) >> 22);
        int vZ = ((value & 0x300000) >> 20);

        int startingX = 2 - (vX * 2);
        int startingZ = vZ * -4;

        sharpTurnValue = (value & 0x0FF0000) >> 16;
        smoothTurnValue = (value & 0x000FF0)>>4;
        backTurnValue = value & 0x000F;

        this.transform.position = new Vector3(startingX, 0, startingZ);
        this.transform.eulerAngles = new Vector3(0, 180, 0);
        record = 0f;
        time = 0f;

        sensor_detected = 0;
        dead = false;
        halfLine = false;

        scoreLines = new bool[6] { false, false, false, false, false, false };
    }

    private void Drive()
    {
        tr.Translate(new Vector3(0, 0, 1) * speed / 10);
    }

    private void Turn(int value_value)
    {
        tr.Rotate(new Vector3(0, 1, 0) * value_value);
    }

    private void MoveCar()
    {
        switch (sensor_detected)
        {
            case 0b000:
                Drive();
                break;
            case 0b100:
                Turn(sharpTurnValue);
                break;
            case 0b110:
                Turn(smoothTurnValue);
                break;
            case 0b011:
                Turn(-smoothTurnValue);
                break;
            case 0b001:
                Turn(-sharpTurnValue);
                break;
            case 0b111:
            case 0b010:
            case 0b101:
                Turn(backTurnValue);
                break;

            case 0b1111:
                //body sensor detected fence
                incomplete();
                break;
        }

        if (time > timeLimit)  // to kill zombie car 
        {
            incomplete();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "FinishLine")
        {
            if (halfLine)
            {
                complete();
            }
            else
            {
                incomplete();
            }
        }

        else if (other.name == "HalfLine")
        {
            this.halfLine = true;
        }

        if(other.tag == "scoreLine")
        {
            scoreLines[int.Parse(other.name[9].ToString()) - 1] = !scoreLines[int.Parse(other.name[9].ToString()) - 1];
        }
    }

    private void complete()
    {
        this.finish = true;
        ScoreCheck();
        record += (100-time);
        this.dead = true;
    }

    private void incomplete()
    {
        this.finish = true;
        ScoreCheck();
        this.dead = true;
    } 

    private void ScoreCheck()
    {
        foreach(bool b in scoreLines)
        {
            if (b == true)
            {
                record += 10;
            }
        }
    }
}
