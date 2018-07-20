using UnityEngine;

/**
 * Used to transform any Monobehavior into a coroutine, by implementing the IWaitForCustom interface and returning itself in a method.
 * See DialogBox.Show for an example
 */
public class WaitForCustom : CustomYieldInstruction {
    private IWaitForCustom waitForCustom;

    public override bool keepWaiting {
        get {
            if (waitForCustom == null) {
                return false;
            } else {
                return !waitForCustom.IsFinished();
            }
        }
    }

    public WaitForCustom(IWaitForCustom waitForCustom) {
        this.waitForCustom = waitForCustom;
    }
}
