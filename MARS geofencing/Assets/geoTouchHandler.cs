using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class geoTouchHandler : MonoBehaviour
{
    public Camera arCamera;
    public GameObject firstPrizeProxy;
    public GameObject firstPrize;
    [SerializeField] GameObject secondPrizeProxy;
    [SerializeField] GameObject secondPrize;
    public GameObject objectPrize;
    public int totalPrizeCount;
    public Text scoreText;
    public Button acquireButton; 
    public AudioSource acquireAudio;
    public AudioSource victoryAudio;
    public GameObject planesVisualiser;
    public GameObject pointCloudVisualiser;
    private Touch touch;
    private Vector3 touchPosition;
    private int scoreCount = 0;
    private string successMessage = "";

    void Awake(){
        if (secondPrizeProxy.activeSelf == true) {
            secondPrizeProxy.SetActive(false);
        }
        if (objectPrize.activeSelf == true) {
            objectPrize.SetActive(false);
        }
        if (Application.isEditor) {
            acquireButton.gameObject.SetActive(true);
            acquireButton.onClick.AddListener(getPrizeOnClick);
        }
    }

    // In Unity Editor only
    void getPrizeOnClick() {
        if (firstPrizeProxy.activeSelf == true) {
            acquirePrize(firstPrizeProxy);
            secondPrizeProxy.SetActive(true);
            return;
        }
        if (secondPrizeProxy.activeSelf == true) {
            acquirePrize(secondPrizeProxy);
            return;
        }
    }

    void Update()
    {
        // Hide mapping visualisers if object has appeared
        if ((firstPrize.activeSelf == true) || ((secondPrizeProxy.activeSelf == true) && (secondPrize.activeSelf == true)) ) {
            planesVisualiser.SetActive(false);
            pointCloudVisualiser.SetActive(false);
        } else {
            planesVisualiser.SetActive(true);
            pointCloudVisualiser.SetActive(true);
        }

        // Get touches on screen
        if (Input.touchCount > 0) {
            touch = Input.GetTouch(0);
            touchPosition = Input.GetTouch(0).position;
        
            // Raycast from first touch
            if (touch.phase == TouchPhase.Began) {
                RaycastHit hit;
                Ray ray = arCamera.ScreenPointToRay(touchPosition);
            // If raycast hits our GameObject
                if (Physics.Raycast(ray, out hit)){
                    if (hit.collider.tag == "Touchable") {
                        string hitName = hit.collider.gameObject.name;
                        if (hitName == firstPrize.name) {
                            acquirePrize(firstPrizeProxy);
                            secondPrizeProxy.SetActive(true);
                        }
                        if (hitName == secondPrize.name) {
                            acquirePrize(secondPrizeProxy);
                        }
                    }
                }
            }
        }

        scoreText.text = "Acquired: " + scoreCount + "/" + totalPrizeCount + successMessage;

        if (scoreCount == totalPrizeCount) {
            scoreText.GetComponent<Text>().color = Color.yellow;
            victoryAudio.PlayOneShot(victoryAudio.clip);
            successMessage = " - GAME COMPLETED! KA MAU TE WEHI!";
        }
    }

    void acquirePrize(GameObject prize){
        scoreCount++;
        prize.SetActive(false);
        acquireAudio.PlayOneShot(acquireAudio.clip);
    }

    void OnDisable(){
        if (Application.isEditor) {
            acquireButton.onClick.RemoveListener(getPrizeOnClick);
        }
    }
}
