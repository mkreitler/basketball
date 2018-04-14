#import <IntegratedBasketball-Swift.h>

// When native code plugin is implemented in .mm / .cpp file, then functions
// should be surrounded with extern "C" block to conform C function naming rules
extern "C" {
    extern ViewController* unityViewController;
    
    void IOS_SetScore (int score)
    {
        if (unityViewController) {
            [unityViewController setScoreWithValue: score];
        }
    }

    void IOS_UnityDidCompleteSetup() {
        if (unityViewController) {
            [unityViewController unityDidCompleteSetup];
        }
    }
}

