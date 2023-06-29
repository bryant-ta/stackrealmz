using System.Collections;
using UnityEngine;

public static class Utils
{
    // Used to generate evenly spaced 2D vectors around a point on the y plane.
    // Returns the *ith* vector of *total* evenly spaced *radius* long 2D Vectors around *center*.
    public static Vector3 GenerateCircleVector(int i, int total, float radius, Vector3 center) {
        float angle = i * Mathf.PI * 2f / total;
        return center + new Vector3(Mathf.Cos(angle) * radius, center.y, Mathf.Sin(angle) * radius);
    }
    
    // Actually have a lot of trouble combining with MoveCardToPoint nicely... leaving separate for now for ease of use
    // prob need to move this out of moveable???
    public static IEnumerator MoveStackToPoint(Stack stack, Vector3 endPoint) {
        Vector3 startPos = stack.transform.localPosition;
        float t = 0f;

        stack.isLocked = true;
        
        while (t < 1) {
            t += Constants.CardMoveSpeed * Time.deltaTime;
            stack.transform.localPosition = Vector3.Lerp(startPos, endPoint, t);
            yield return null;
        }
        
        stack.isLocked = false;
    }
    
    public static IEnumerator MoveCardToPoint(Card card, Vector3 endPoint) {
        Vector3 startPos = card.transform.localPosition;
        float t = 0f;

        card.mStack.isLocked = true;
        
        while (t < 1) {
            t += Constants.CardMoveSpeed * Time.deltaTime;
            card.transform.localPosition = Vector3.Lerp(startPos, endPoint, t);
            yield return null;
        }
        
        card.mStack.isLocked = false;
    }
}
