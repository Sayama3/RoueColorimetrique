using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;
using yutokun;
using Random = UnityEngine.Random;


public class Visualizer : MonoBehaviour
{
    [SerializeField] private Shader m_Shader;
    [SerializeField] private string m_CSVPath;
    [SerializeField, Min(0)] private float m_RadiusMin = 5;
    [SerializeField, Min(0)] private float m_RadiusScaler = 1;
    [SerializeField] private float m_ImageSize = 0.1f;
    
    private const int c_DominateHueIndex = 6;
    private List<Image> m_Images;
    private static readonly int MainTex = Shader.PropertyToID("_MainTex");
    private static readonly int MainColor = Shader.PropertyToID("_Color");
    private const int c_AverageHueIndex = 3;
    private const int c_NamesIndex = 0;

    private void PrepareFromCsv()
    {
        List<List<string>> csv = CSVParser.LoadFromPath(Path.Combine(Application.streamingAssetsPath, m_CSVPath), Delimiter.Comma);
        // Line 0 is the header.
        {
            Debug.Log("Names :" + csv[0][c_NamesIndex]);
            Debug.Log("Average Hue Index :" + csv[0][c_AverageHueIndex]);
            Debug.Log("Dominate Hue Index :" + csv[0][c_DominateHueIndex]);
        }
        m_Images ??= new(csv.Count);
        for (int line = 1; line < csv.Count; line++)
        {
            m_Images.Add(new Image(csv[line][c_NamesIndex], csv[line][c_DominateHueIndex], csv[line][c_AverageHueIndex]));
        }
        
        Debug.Log("Parsing of necessary data finished");
    }

    private void PreprocessData()
    {
        m_Images.Sort();
        Debug.Log("Images Sorted.");
        // foreach (var image in m_Images)
        // {
            // Debug.Log($"{image.Name}: d={image.DominantHue}, a={image.AverageHue}");
        // }
    }

    /// <summary>
    /// Return a texture2D from the resources folders
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    Texture2D GetTexture(string name)
    {
        string resourceStr = "images/" + name;
        resourceStr = resourceStr.Remove(resourceStr.Length - 4, 4);
        Texture2D tex = Resources.Load<Texture2D>(resourceStr);
        return tex;
    }
    
    // Texture2D GetTexture(string name)
    // {
    //     string imgPath = Path.Combine(Application.streamingAssetsPath, "images/" + name);
    //     Texture2D tex = new Texture2D(1, 1);
    //     try
    //     {
    //         byte[] imgtxt = File.ReadAllBytes(imgPath);
    //         bool result = ImageConversion.LoadImage(tex, imgtxt);
    //         if (!result) return null;
    //     }
    //     catch (Exception e)
    //     {
    //         Debug.LogException(e);
    //         return null;
    //     }
    //     return tex;
    // }
    
    private void GenerateImages()
    {
        Debug.Log("Start Image Generation");
        for (int i = 0; i < m_Images.Count; i++)
        {
            var image = m_Images[i];
            Texture tex = GetTexture(image.Name);
            
            var degree = ((float)i / (float)m_Images.Count) * (360.0f);
            Quaternion rot = Quaternion.AngleAxis(degree, Vector3.up);
            Vector3 fwd = rot * Vector3.forward;
            

            float distance = Mathf.Lerp(m_RadiusMin, m_Images.Count / (2*Mathf.PI) + m_RadiusMin, image.AverageHue);
            // float distance = Mathf.Lerp(m_Images.Count / (2*Mathf.PI) - m_RadiusMin, m_Images.Count / (2*Mathf.PI) + m_RadiusMin, image.DominantHue - image.AverageHue);
            fwd *= distance * m_RadiusScaler;
            // fwd *= m_Images.Count / (2*Mathf.PI);
            var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            quad.gameObject.name = image.Name;
            quad.transform.rotation = Quaternion.LookRotation(Vector3.down, rot * Vector3.forward);
            fwd.y += Random.value;
            quad.transform.position = fwd;
            quad.transform.localScale = new Vector3(m_ImageSize, m_ImageSize, m_ImageSize);

            quad.GetComponent<MeshRenderer>().material = new Material(m_Shader);
            if (tex != null)
            {
                quad.GetComponent<MeshRenderer>().material.SetTexture(MainTex, tex);
            }
            else
            {
                quad.GetComponent<MeshRenderer>().material.SetColor(MainColor, Color.HSVToRGB(image.DominantHue, 1, 1));
            }


            quad.transform.SetParent(this.transform, true);
        }
        Debug.Log("Image generated");
    }
    
    private void Start()
    {
        PrepareFromCsv();
        
        PreprocessData();

        GenerateImages();
    }
    
    

    // Update is called once per frame
    void Update()
    {
    }
}
