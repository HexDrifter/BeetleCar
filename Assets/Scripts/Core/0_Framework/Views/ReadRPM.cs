using UnityEngine;
using Beetle.Framework;
using UnityEngine.UI;
using TMPro;
using Beetle.SystemUtilities;
using Beetle.Domain;

public class ReadRPM : MonoBehaviour
{
    public TextMeshProUGUI rpmText;
    public CarBehavior car;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rpmText.text = "RPM:" + car.engineRPM;
    }
}
