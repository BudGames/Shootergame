using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{

    public CharacterController CH;
    public Joystick joystick;

    public float MouseX;
    public float MouseY;
    public Transform Head;
    public Transform Body;
    float Angle;

    public Camera Cam;

    public GameObject GunFire_Prefab;
    public GameObject Shell_Prefab;
    public GameObject ImpactPoint_Prefab;
    public Transform ShellPosition;

    public AudioSource ShootingEffect;


    public Vector3 Gravity;
    public Transform groundCheck;
    public LayerMask groundMask;
    bool isGrounded;

    // Start is called before the first frame update
    void Start()
    {
        
    }


    void LateUpdate() 
    {
        if(Input.touchCount > 0)
        {
            if(Input.touchCount == 1)
            {
                if(Input.GetTouch(0).position.x > Screen.width/2)
                {
                    ChangeView(0);
                }
            }
            if(Input.touchCount == 2)
            {
                if(Input.GetTouch(0).position.x > Screen.width/2)
                {
                    ChangeView(0);
                }

                if(Input.GetTouch(1).position.x > Screen.width/2)
                {
                    ChangeView(1);
                }
            }
        }
    }


    public void ChangeView(int number)
    {
        MouseX = Input.GetTouch(number).deltaPosition.x * 10 * Time.deltaTime;
        Body.Rotate(Vector3.up , MouseX);

        MouseY = Input.GetTouch(number).deltaPosition.y * 10 * Time.deltaTime;
        Angle -= MouseY;
        Angle = Mathf.Clamp(Angle , -30 , +45);
        Head.localRotation = Quaternion.Euler(Angle , 0 , 0);
    }


    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position , 0.4f , groundMask);

        if(isGrounded && Gravity.y < 0)
        {
            Gravity.y = -2f;
        }

        Gravity.y += -9.18f * Time.deltaTime;
        CH.Move(Gravity * Time.deltaTime);

        Vector3 Move = transform.right * joystick.Horizontal * 10 * Time.deltaTime + transform.forward * joystick.Vertical * 10 * Time.deltaTime;
        CH.Move(Move);
    }

    public void Down(BaseEventData eventData)
    {
        InvokeRepeating("Shooting" , 0 , 0.1f);
    }

    public void Shooting()
    {
        RaycastHit HIT;

        GetComponent<Animator>().SetTrigger("Shooting");

        ShootingEffect.Play();
        
        GunFire_Prefab.GetComponent<ParticleSystem>().Play();

        if(Physics.Raycast(Cam.transform.position , Cam.transform.forward , out HIT , 100))
        {
            Enemy enemy = HIT.transform.GetComponent<Enemy>();
            if(enemy != null)
            {
                //enemy.Damage();
            }

            GameObject A = Instantiate(ImpactPoint_Prefab , HIT.point , Quaternion.LookRotation(HIT.normal));
            GameObject B = Instantiate(Shell_Prefab , ShellPosition.position , Quaternion.identity);
            Destroy(A , 2);
            Destroy(B , 2);
        }
        
    }

    public void Up(BaseEventData eventData)
    {
        CancelInvoke("Shooting");
    }
}
