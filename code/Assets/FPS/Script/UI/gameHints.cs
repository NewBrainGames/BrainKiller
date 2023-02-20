using UnityEngine;

namespace FPS.Script.UI
{
    public class gameHints
    {
        private string[] hints = new []
        {
            "提示：按下“F”键可以隐藏原始神经元图像", 
            "提示：按下“K”键可以隐藏绿色神经元结构",
            "提示：按下“R”键可以补充弹药",
            // "小提示：按下“Q”键可以切换到蓝色枪支，标记错误分叉点"
        };

        private int hintsIndex = 0;

        public string getHint()
        {
            int length = hints.Length;
            int i = hintsIndex % length;
            hintsIndex += 1;
            return hints[i];
        }
        
    }
}