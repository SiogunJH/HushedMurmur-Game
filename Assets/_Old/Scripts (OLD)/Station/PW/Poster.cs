using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Poster : MonoBehaviour
{
    [SerializeField] Bird.Controller bird;

    [SerializeField] TextMeshProUGUI birdName;
    [SerializeField] TextMeshProUGUI repellant;
    [SerializeField] TextMeshProUGUI mainTrait;
    [SerializeField] TextMeshProUGUI secondaryTrait;
    [SerializeField] TextMeshProUGUI favoredRoom;

    // Start is called before the first frame update
    void Start()
    {
        birdName.text = Enum.GetName(typeof(Bird.Type), bird.birdType).ToUpper();
        repellant.color = bird.repellantColor;
        // mainTrait.text = Enum.GetName(typeof(Bird.Trait), bird.mainTrait).ToUpper();
        // secondaryTrait.text = Enum.GetName(typeof(Bird.Trait), bird.secondaryTrait).ToUpper();
        // favoredRoom.text = Enum.GetName(typeof(Room.Type), bird.favoredRoom).ToUpper();
    }
}
