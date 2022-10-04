using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class XML
{
    public int[] getOffsets(int num)
    {
        int[] value = new int[2];
        TextAsset textAsset = GetTextAsset(num);
        if (textAsset)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(textAsset.text);
            XmlNodeList nodeList = xmlDocument.GetElementsByTagName("section");
            foreach (XmlNode node in nodeList)
            {
                value[0] = int.Parse(node.Attributes["startOffset"].Value);
                value[1] = int.Parse(node.Attributes["endOffset"].Value);
            }
        }
        return value;
    }

    public static List<int[]> GetRows(int num)
    {
        List<int[]> values = new List<int[]>();
        TextAsset textAsset = GetTextAsset(num);
        if (textAsset)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(textAsset.text);
            XmlNodeList nodeList = xmlDocument.GetElementsByTagName("row");
            foreach (XmlNode node in nodeList)
            {
                int[] value = new int[2];
                value[0] = int.Parse(node.Attributes["offset"].Value);
                value[1] = int.Parse(node.Attributes["count"].Value);
                values.Add(value);
            }
        }
        return values;
    }

    private static TextAsset GetTextAsset(int num)
    {
        string path = "GenerateScene/";
        path += "Scene_" + num;
        return Resources.Load<TextAsset>(path);
    }

}
