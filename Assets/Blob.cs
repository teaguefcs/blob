﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Blob : MonoBehaviour
{

    Rigidbody2D blob;
    bool isOnGround = true;
    public int blobSize = 1;
    public float blobSizeMod = 1;
    public static ArrayList currentBlobs;
    public GameObject controller;
    bool paused = false;
    float timer;
    BlobTexture texture;
    AudioSource source;
    Camera cam;
    GameObject blobPrefab;
    AudioClip jump, land;
    private Animator animator;

    void Start()
    {
        blob = GetComponent<Rigidbody2D>();
        cam = FindObjectOfType<Camera>();
        animator = GetComponent<Animator>();
        source = GetComponent<AudioSource>();
        texture = GetComponentInChildren<BlobTexture>();
        blobPrefab = (GameObject)Resources.Load("Blob");
        jump = (AudioClip)Resources.Load("Jump");
        land = (AudioClip)Resources.Load("Land");
        currentBlobs = new ArrayList(GameObject.FindGameObjectsWithTag("Blob"));
        controller = GameObject.FindGameObjectWithTag("Controller");
    }
    
    void Update()
    {
        if (tag == "Active" && timer <= 0)
        {
            if (Input.GetKeyDown("w") && isOnGround)
            {
                blob.AddForce(new Vector2(0, 32), ForceMode2D.Impulse);
                animator.SetTrigger("Jump");
                source.PlayOneShot(jump);
            }
            if (Input.GetMouseButtonDown(0) && blobSize > 1)
            {
                Vector2 vector = cam.ScreenToWorldPoint(Input.mousePosition) - transform.position;
                vector.Normalize();
                GameObject newBlob = Instantiate(blobPrefab, GetComponent<Transform>().position, GetComponent<Transform>().rotation);
                newBlob.GetComponent<Rigidbody2D>().AddForce(vector * 50, ForceMode2D.Impulse);
                DecreaseSize(1);
                currentBlobs.Add(newBlob);
            }
            if (Input.GetKeyDown(KeyCode.Space) && currentBlobs.Count != 0)
            {
                GameObject nextBlob = (GameObject)currentBlobs[0];
                currentBlobs.Remove(nextBlob);
                currentBlobs.Add(gameObject);
                nextBlob.tag = "Active";
                nextBlob.GetComponent<Blob>().timer = .2f;
                tag = "Blob";
            }
            if (!isOnGround)
            {
                float angle;
                if (GetComponent<Rigidbody2D>().velocity.x > 0) {
                    angle = -Vector2.Angle(GetComponent<Rigidbody2D>().velocity, Vector2.up);
                }
                else
                {
                    angle = Vector2.Angle(GetComponent<Rigidbody2D>().velocity, Vector2.up);
                }
                texture.transform.rotation = Quaternion.Euler(0, 0, angle);
            }
            else
            {
                texture.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (paused)
                {
                    controller.GetComponent<GameScript>().Unpause();
                }
                else
                {
                    controller.GetComponent<GameScript>().Pause();
                }
            }
        }
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        if (GetComponent<Rigidbody2D>().velocity.x > 0 && isOnGround)
        {
            animator.SetInteger("State", 1);
        }
        else if(GetComponent<Rigidbody2D>().velocity.x < 0 && isOnGround)
        {
            animator.SetInteger("State", -1);
        }
        else
        {
            animator.SetInteger("State", 0);
        }
        if (tag == "Active" && !GetComponent<AudioSource>().isPlaying)
        {
            if (Input.GetKey("a"))
            {
                source.Play();
            }
            if (Input.GetKey("d"))
            {
                source.Play();
            }
        }
        if (Input.GetKeyUp("a") && !Input.GetKey("d"))
        {
            source.Pause();
        }
        if (Input.GetKeyUp("d") && !Input.GetKey("a"))
        {
            source.Pause();
        }
        if (Input.GetKey("d") && Input.GetKey("a"))
        {
            source.Pause();
        }
        if (isOnGround && !Input.GetKey("a") && !Input.GetKey("d"))
        {
            source.Pause();
        }
    }

    private void FixedUpdate()
    {
        if (tag == "Active")
        {
            if (Input.GetKey("a"))
            {
                blob.AddForce(new Vector2(-1, 0), ForceMode2D.Impulse);
            }
            if (Input.GetKey("d"))
            {
                blob.AddForce(new Vector2(1, 0), ForceMode2D.Impulse);
            }
        }
    }

    public void SetTimer(float time)
    {
        timer = time;
    }
    
    void OnCollisionStay2D(Collision2D c)
    {
        if (c.gameObject.CompareTag("Floor") || c.gameObject.CompareTag("Button"))
        {
            isOnGround = true;
            texture.GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<SpriteRenderer>().enabled = true;
            animator.SetTrigger("Land");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor") || collision.gameObject.CompareTag("Button"))
        {
            isOnGround = true;
            texture.GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<SpriteRenderer>().enabled = true;
            animator.SetTrigger("Land");
            source.PlayOneShot(land);
        }
        if (collision.gameObject.CompareTag("Hazard"))
        {
            if (currentBlobs.Count > 0)
            {
                if (CompareTag("Active"))
                {
                    GameObject nextBlob = (GameObject)currentBlobs[0];
                    currentBlobs.Remove(nextBlob);
                    nextBlob.tag = "Active";
                    nextBlob.GetComponent<Blob>().timer = .2f;
                }
                Destroy(this.gameObject);
            }   
            else
            {
                controller.GetComponent<GameScript>().Lose();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("VictoryZone"))
        {
            if (CompareTag("Blob"))
            {
                GameObject nextBlob = gameObject;
            }
            controller.GetComponent<GameScript>().Win();
        }
    } 

    void OnCollisionExit2D(Collision2D c)
    {
        if (c.gameObject.CompareTag("Floor") || c.gameObject.CompareTag("Button"))
        {
            isOnGround = false;
            texture.GetComponent<SpriteRenderer>().enabled = true;
            GetComponent<SpriteRenderer>().enabled = false;
            animator.ResetTrigger("Land");
        }
    }

    public void IncreaseSize(int amt)
    {
        Vector3 scale = transform.localScale;
        for (int i = blobSize; i < blobSize + amt; i++)
        {
            blobSizeMod = 1 / (float)(i + 3);
            transform.localScale = new Vector3(scale.x + blobSizeMod, scale.y + blobSizeMod, scale.z);
            GetComponent<Rigidbody2D>().mass += blobSizeMod;
            scale = transform.localScale;
        }
        blobSize += amt;
    }

    public void DecreaseSize(int amt)
    {
        if (blobSize > 1) {
            Vector3 scale = transform.localScale;
            for (int i = blobSize; i > blobSize - amt; i--)
            {
                blobSizeMod = 1 / (float)(i + 2);
                transform.localScale = new Vector3(scale.x - blobSizeMod, scale.y - blobSizeMod, scale.z);
                GetComponent<Rigidbody2D>().mass -= blobSizeMod;
                scale = transform.localScale;
            }
            blobSize -= amt;
        }
    }

    private void OnDestroy()
    {
        currentBlobs.Remove(gameObject);
    }
}