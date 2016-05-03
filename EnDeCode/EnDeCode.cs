using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;
using System.Security.Cryptography;

namespace EnDeCode
{
    public class EnDeCode
    {
        /// <summary>
        /// DES加密算法
        /// sKey为8位或16位
        /// </summary>
        /// <param name="pToEncrypt">需要加密的字符串</param>
        /// <param name="sKey">密钥</param>
        /// <returns></returns>
        public string DesEncrypt(string pToEncrypt, string sKey)
        {
            byte[] iKeys = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            //byte[] inputByteArray = Encoding.UTF8.GetBytes(pToEncrypt.Substring(0, 8));
            //byte[] keys= Encoding.UTF8.GetBytes(sKey.Substring(0, 8));
            //des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            //des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
            byte[] keys = new byte[sKey.Length / 2];
            byte[] inputByteArray = new byte[pToEncrypt.Length / 2];
            for (int i = 0; i < pToEncrypt.Length / 2; i++)
            {
                inputByteArray[i] = Convert.ToByte(pToEncrypt.Substring(i * 2, 2), 16);
                //keys[i] = Convert.ToByte(sKey.Substring(i * 2, 2), 16);
            }
            for (int i = 0; i < sKey.Length / 2; i++)
            {
                //inputByteArray[i] = Convert.ToByte(pToEncrypt.Substring(i * 2, 2), 16);
                keys[i] = Convert.ToByte(sKey.Substring(i * 2, 2), 16);
            }
            des.Key = keys;
            des.IV = keys;
            des.IV = iKeys;
            des.Mode = CipherMode.ECB;
            des.Padding = PaddingMode.Zeros;
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            StringBuilder ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }
            //ret.ToString();
            //return Convert.ToBase64String(ms.ToArray());
            return ret.ToString();
            //return a;
        }
        /// <summary>
        /// DES解密算法
        /// sKey为8位或16位
        /// </summary>
        /// <param name="pToDecrypt">需要解密的字符串</param>
        /// <param name="sKey">密钥</param>
        /// <returns></returns>
        public string DesDecrypt(string pToDecrypt, string sKey)
        {
            byte[] iKeys = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.Mode = CipherMode.ECB;
            des.Padding = PaddingMode.Zeros;
            byte[] keys = new byte[sKey.Length / 2];
            byte[] inputByteArray = new byte[pToDecrypt.Length / 2];
            for (int i = 0; i < pToDecrypt.Length / 2; i++)
            {
                inputByteArray[i] = Convert.ToByte(pToDecrypt.Substring(i * 2, 2), 16);
                //keys[i] = Convert.ToByte(sKey.Substring(i * 2, 2), 16);
            }
            for (int i = 0; i < sKey.Length / 2; i++)
            {
                //inputByteArray[i] = Convert.ToByte(pToEncrypt.Substring(i * 2, 2), 16);
                keys[i] = Convert.ToByte(sKey.Substring(i * 2, 2), 16);
            }
            //for (int x = 0; x < 8; x++)
            //{
            //    int i = (Convert.ToInt32(pToDecrypt.Substring(x * 2, 2), 16));
            //    inputByteArray[x] = (byte)i;
            //}
            //des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            //des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
            des.Key = keys;
            des.IV = keys;
            des.IV = iKeys;
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            StringBuilder ret = new StringBuilder();
            //return System.Text.Encoding.Default.GetString(ms.ToArray());
            foreach (byte b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }
            return ret.ToString();
        }

        public static string EncryptDES(string encryptString, string encryptKey)
        {
            byte[] Keys = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
            try
            {
                byte[] skeys = new byte[8];
                for (int i = 0; i < 8; i++)
                {
                    skeys[i] = Convert.ToByte(encryptKey.Substring(i * 2, 2), 16);
                }
                //byte[] rgbKey = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
                byte[] rgbKey = skeys;
                byte[] rgbIV = Keys;
                byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
                DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Convert.ToBase64String(mStream.ToArray());
            }
            catch
            {
                return encryptString;
            }
        }

        public string GetASCII(string OriNum)
        {
            string strResult = "";
            for (int i = 0; i < OriNum.Length; i++)
            {
                int intCharNum = Convert.ToInt32(OriNum[i]);
                strResult = strResult + "00" + intCharNum.ToString();
            }
            return strResult;
        }

        public string GetMD5(string original)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] b = md5.ComputeHash(Encoding.UTF8.GetBytes(original));
            return BitConverter.ToString(b).Replace("-", string.Empty);
        }

        public string GetXOR(string OriString)
        {
            string strResult = "";
            //string strHexString = ConvertString(OriString, 16, 2);
            //byte[] bStr = (new UnicodeEncoding()).GetBytes(OriString);
            byte[] byteHex = strToToHexByte(OriString);
            byte byteResult = Convert.ToByte(byteHex[0] ^ byteHex[0]);
            for (int i = 0; i < byteHex.Length - 1; i++)
            {
                if (i == 0)
                {
                    byteResult = Convert.ToByte(byteHex[0] ^ byteHex[i + 1]);
                }
                else
                {
                    byteResult = Convert.ToByte(byteResult ^ byteHex[i + 1]);
                }
            }
            strResult = Convert.ToString(byteResult, 2);
            if (strResult.Length < 8)
            {
                for (int i = 1; i <= 8 - strResult.Length; i++)
                {
                    strResult = "0" + strResult;
                }
            }
            return strResult;
        }

        public byte[] strToToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        public string GetHexString(int MaxNum)
        {
            String digits = "0123456789ABCDEF0123456789ABCDEF";
            String result = "";
            Random rnd = new Random();
            int r1 = rnd.Next(11, 14);            
            while (result.Length < MaxNum)
            {
                rnd = new Random(r1 * unchecked((int)DateTime.Now.Ticks) + result.Length);
                int index = rnd.Next(digits.Length);
                char ch = digits[index];
                if (result.Length == 0 || result[result.Length - 1] != ch)
                {
                    result += ch;
                    digits = digits.Remove(index, 1);
                }
            }
            return result;
        }
    }
}
