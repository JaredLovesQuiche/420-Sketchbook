using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
    public Text text;
    public float secondsLeft = 60.0f;
    public bool hasTriggeredBathroom = false;

    public BoxCollider bathroomTrigger;

    void Update()
    {
        if (hasTriggeredBathroom)
        {
            text.text = "You made it to the bathroom!!!";
            return;
        }

        secondsLeft -= Time.deltaTime;
        if (secondsLeft <= 0)
        {
            text.text = "You didnt make it to the bathroom...";
        }
        else
            text.text = "You have " + Mathf.RoundToInt(secondsLeft).ToString() + " seconds left before you pee your pants!";
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            hasTriggeredBathroom = true;
        }
    }
}
