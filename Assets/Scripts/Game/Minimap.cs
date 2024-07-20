using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class MiniMap : NetworkBehaviour
{
    [SerializeField] private Shader shader;
    [SerializeField] private Camera miniCamera;
    private RenderTexture renderTexture;


    public override void OnNetworkSpawn()
    {
        Debug.Log("OnNetworkSpawn called");
        if (!IsOwner) return;
        renderTexture = new RenderTexture(256, 256, 16, RenderTextureFormat.ARGB32);
        renderTexture.Create();

        renderTexture.Release();

        miniCamera.targetTexture = renderTexture;

        Material material = new Material(shader);
        material.mainTexture = renderTexture;

        GameObject miniMapImageGO = GameObject.FindGameObjectWithTag("MiniMap");
        Debug.Log("MiniMap: " + miniMapImageGO.transform.position.x);
        miniMapImageGO.GetComponent<Image>().material = material;
    }
}