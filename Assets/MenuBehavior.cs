using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuBehavior : MonoBehaviour
{
    private int pageNum;
    public GameObject[] pages;

    // Start is called before the first frame update
    void Start()
    {
        pageNum = 0;
        pages[pageNum].SetActive(true);
    }

    public void Apply(string action) {
        pages[pageNum].SetActive(false);
        if (action == "prev" && pageNum != 0) {
            pageNum -= 1;
        } else if (action == "next" && pageNum != pages.Length-1) {
            pageNum += 1;
        }
        pages[pageNum].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
