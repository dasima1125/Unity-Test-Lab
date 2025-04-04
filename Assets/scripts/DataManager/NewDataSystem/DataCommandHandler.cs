using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataCommandHandler 
{
    // Start is called before the first frame update
    private readonly DataController controller;

    public DataCommandHandler(DataController controller)  // 생성자로 주입
    {
        this.controller = controller;
    }
}
