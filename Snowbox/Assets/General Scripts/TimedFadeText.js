var liveTime = 5.0;            // The number of seconds the GUIText will last before starting to fade
var fadeTime = 2.0;            // The number of seconds to fade until totally transparent
private var time = 0.0;        // Static var to track how much time has passed
private var isFading = false;  // Static var to track if we're in the fading stage
private var startAlpha = 1.0;  // Static var to keep track of the initial amount of alpha

function Start () {
    // This script uses the GUIText's material to set the alpha fade.
    // If the font doesn't have a material, then this won't work, so disable the script.
    if (!guiText.material) {
        enabled = false;
    }
    
    // Get the starting alpha value.
    // If the developer has the text start transparent, then we need to fade from that point.
    startAlpha = guiText.material.color.a;
}

function Update () {
    // Update our time var to keep track of how much time has passed.
    time += Time.deltaTime;
    
    if (isFading) {
        // We're in the fading stage.  If we've reached the end of this stage, then destroy the gameObject.
        if (time >= fadeTime) {
            Destroy(gameObject);
        }
        
        // We're still fading, so update the material's alpha color to make it fade a little more.
        guiText.material.color.a = CalculateAlpha();
    }
    else if (time >= liveTime) {
        // If we're not fading yet, but should be, then update our values to proceed to the fading stage.
        isFading = true;
        time = 0.0;
    }
    
    // If we're not fading yet, and don't need to be yet, then nothign will happen at this point.  The
    //   text will just exist, and the timer will keep incrementing until there's a state change.
}

// CalculateAlpha() simple takes the static global vars we're using to keep track of everything
//   to figure out our current alpha value from 0 to 1.
function CalculateAlpha() {
    // Find out the percent of time from 0 to 1 that has gone between when the text starts and stops fading
    var timePercent = Mathf.Clamp01((fadeTime - time) / fadeTime);
    // Generate a nice, smooth value from 1 to 0 to represent how faded the text is
    var smoothAlpha = Mathf.SmoothStep(0.0, startAlpha, timePercent);
    
    // We actually could just return the timePercent value for a linear fade, but we want it to be smooth,
    //  so return the smoothAlpha.
    return smoothAlpha;
}

@script RequireComponent(GUIText)
 
