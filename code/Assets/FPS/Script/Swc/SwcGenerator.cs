using Unity.VisualScripting;
using UnityEngine;

class SwcGenerator : CommonSwcGenerator{
    protected override void Start()
    {
        base.Start();
        base.imagePosition = new Vector3(70,9,101);
        base.drawSWC(base.GetCurrentImageIndex());
    }

    protected override void Update()
    {
        base.Update();
    }
}
