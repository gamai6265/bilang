using System.Text.RegularExpressions;
using LitJson;

namespace Cloth3D.Ui {
    internal class LoginUtils {
        public static bool IsHandset(string str) {
            return Regex.IsMatch(str, @"^[1]+[3,5,8]+\d{9}");
        }
    }
}