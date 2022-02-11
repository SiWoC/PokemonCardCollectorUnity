using Assets.Scripts.Classes.Globals;
using Globals;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class PackWrapperWithMesh : MonoBehaviour, IDragHandler, IEndDragHandler
{

    public GameObject[] frames;
    public GameObject swipeDownFrame; // with tearoff completely out of view

    private int generation;
    private float easing = 1.5f;
    //private MeshFilter meshFilter;
    private SkinnedMeshRenderer skinedMeshRenderer;
    private bool opened = false;
    private int frame = 0;
    private int framesHandled = 0;
    private float stepWidthCoverage = 0.6f;
    private int stepWidth;

    // Start is called before the first frame update
    void Start()
    {
        Pack pack = GetComponentInParent<Pack>();
        generation = pack.generation;
        //meshFilter = GetComponent<MeshFilter>();
        skinedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        skinedMeshRenderer.material = GenerateMaterial();

        // Getting meshRendering in front of sprite rendering of cards
        skinedMeshRenderer.sortingLayerName = "Default";
        skinedMeshRenderer.sortingOrder = 25;

        stepWidth = (int)((Screen.width / frames.Length) * stepWidthCoverage * -1);
    }

    private Material GenerateMaterial()
    {
        //Material material = new Material(Shader.Find("Sprites/Default"));
        Material material = new Material(Shader.Find("Unlit/Transparent Cutout"));
        material.mainTexture = Resources.Load<Texture>("Images/Packs/" + PlayerStats.GetNextPack(generation)); // zonder png !!!
        return material;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!opened)
        {
            int numberOfFrames = (int)((eventData.pressPosition.x - eventData.position.x) / stepWidth);
            AddFrame(numberOfFrames);
            if (!PlayerStats.GetTutorialCompleted(TutorialStep.OpenPack))
            {
                PlayerStats.SetTutorialCompleted(TutorialStep.OpenPack);
            }
        }
    }

    private void AddFrame(int numberOfFrames)
    {
        if (numberOfFrames > framesHandled)
        {
            frame = Math.Min(frame + numberOfFrames - framesHandled, frames.Length - 1);
            framesHandled = numberOfFrames;
            skinedMeshRenderer.sharedMesh = frames[frame].GetComponentInChildren<MeshFilter>().sharedMesh;
        }
    }

    public void OnEndDrag(PointerEventData data)
    {
        if (frame == frames.Length - 1)
        {
            opened = true;
            // getting tearoff completely out of view
            skinedMeshRenderer.sharedMesh = swipeDownFrame.GetComponentInChildren<MeshFilter>().sharedMesh;
            StartCoroutine(SmoothMove(transform.position, new Vector3(transform.position.x, -100, transform.position.z)));
            GetComponentInParent<Pack>().Opened();
        }
        framesHandled = 0;
    }

    IEnumerator SmoothMove(Vector3 startpos, Vector3 endpos)
    {
        float t = 0f;
        while (t <= 1.0)
        {
            t += Time.deltaTime / easing;
            transform.position = Vector3.Lerp(startpos, endpos, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }
    }
}
