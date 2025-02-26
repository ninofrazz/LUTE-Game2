using UnityEngine;

[OrderInfo("test ",
              "test",
              "")]
[AddComponentMenu("")]
public class test : Order
{
    public int test1;
    public override void OnEnter()
    {
        //this code gets executed as the order is called
        //some orders may not lead to another node so you can call continue if you wish to move to the next order after this one   
        //Continue();
    }

    public override string GetSummary()
    {
        //you can use this to return a summary of the order which is displayed in the inspector of the order
        return "";
    }
}