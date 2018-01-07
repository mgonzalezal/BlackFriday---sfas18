using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum CheckoutStatus { PlaceItem, PayItem, Done };

public class CheckoutScript : MonoBehaviour {

    GameObject item_to_checkout;
    GameObject checkout_position;
    ParticleSystem particles_checkout_;

    public float time_to_place_item;
    public float time_to_pay;

    float time_to_place_item_left;
    float time_to_pay_left;

    CheckoutStatus check_out_status_;

    Vector3 position_start_;
    Vector3 scale_start_;
    Quaternion rotation_start_;
    
    Vector3 scale_dest_;

    // Use this for initialization
    void Start () {
        checkout_position = transform.parent.Find("CheckOutPoint").gameObject;
        particles_checkout_ = transform.parent.Find("CheckOutParticles").gameObject.GetComponent<ParticleSystem>();
    }
	
	// Update is called once per frame
	void Update () {
        if (item_to_checkout)
        {
            switch (check_out_status_)
            {
                case CheckoutStatus.PlaceItem:
                    {
                        Debug.Log(checkout_position.transform.position);

                        item_to_checkout.transform.position = Vector3.Lerp(position_start_, checkout_position.transform.position, 1.0f - (time_to_place_item_left / time_to_place_item));
                        item_to_checkout.transform.rotation = Quaternion.Lerp(rotation_start_, checkout_position.transform.rotation, 1.0f - (time_to_place_item_left / time_to_place_item));
                        item_to_checkout.transform.localScale = Vector3.Lerp(scale_start_, scale_dest_, 1.0f - (time_to_place_item_left / time_to_place_item));
                        time_to_place_item_left -= Time.deltaTime;
                        if (time_to_place_item_left <= 0.0f)
                        {
                            check_out_status_++;
                        }
                        break;
                    }
                case CheckoutStatus.PayItem:
                    {
                        time_to_pay_left -= Time.deltaTime;
                        if (time_to_pay_left <= 0.0f)
                        {
                            check_out_status_++;
                        }
                        break;
                    }
                case CheckoutStatus.Done:
                    {
                        particles_checkout_.Play();
                        Destroy(item_to_checkout);
                        item_to_checkout = null;
                        break;
                    }
            }
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !item_to_checkout)
        {
            PlayerPickUpScript pick_up = other.gameObject.GetComponent<PlayerPickUpScript>();
            item_to_checkout = pick_up.CheckoutItem();
            if (item_to_checkout) {
                time_to_place_item_left = time_to_place_item;
                time_to_pay_left = time_to_pay;
                check_out_status_ = CheckoutStatus.PlaceItem;
                position_start_ = item_to_checkout.transform.position;
                rotation_start_ = item_to_checkout.transform.rotation;
                scale_start_ = item_to_checkout.transform.localScale;

                scale_dest_ = item_to_checkout.GetComponent<PickUpObjectBehaviour>().GetStartScale();
            }
        }
    }
}
