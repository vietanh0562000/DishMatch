using System.Collections;
using SweetSugar.Scripts;
using SweetSugar.Scripts.Core;
using SweetSugar.Scripts.Items;
using UnityEngine;
using UnityEngine.Serialization;

public class HandTutorial : MonoBehaviour
{
    [FormerlySerializedAs("tutorialManager")] public MTutorialManager mTutorialManager;
    private MItem _tipMItem;
    private Vector3 vDirection;

    void OnEnable()
    {
        _tipMItem = AI.THIS.TipMItem;
        _tipMItem.tutorialUsableItem = true;
        MLevelManager.THIS.tutorialTime = true;
        vDirection = AI.THIS.vDirection;
        PrepareAnimateHand();
    }

    void PrepareAnimateHand()
    {
        var positions = mTutorialManager.GetItemsPositions();
        StartCoroutine(AnimateHand(positions));
    }

    IEnumerator AnimateHand(Vector3[] positions)
    {
        float speed = 1;
        var posNum = 0;

        //		for (int i = 0; i < 2; i++) {
        var i = 0;
        if (AI.THIS.currentCombineType == CombineType.VShape)
            i = 1;
        transform.position = _tipMItem.transform.position;
        posNum++;
        var offset = new Vector3(0.5f, -.5f);
        Vector2 startPos = transform.position + offset;
        Vector2 endPos = transform.position + vDirection + offset;
        var distance = Vector3.Distance(startPos, endPos);
        float fracJourney = 0;
        var startTime = Time.time;

        while (fracJourney < 1)
        {
            var distCovered = (Time.time - startTime) * speed;
            fracJourney = distCovered / distance;
            transform.position = Vector2.Lerp(startPos, endPos, fracJourney);
            yield return new WaitForFixedUpdate();
        }
        yield return new WaitForFixedUpdate();
        PrepareAnimateHand();
    }
}
