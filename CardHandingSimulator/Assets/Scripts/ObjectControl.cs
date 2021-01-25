using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectControl
{
    /// <summary>
    /// 오브젝트의 Scale을 변경하는 코루틴.
    /// time 시간동안 obj.transform.localScale을 scale로 바꾼다.
    /// </summary>
    public static IEnumerator ChangeSizeC(float time, Vector3 scale, GameObject obj)
    {
        Vector3 start = obj.transform.localScale;
        Vector3 speed = (scale - start) / time;
        float curtime = 0.0f;

        while (curtime < time)
        {
            obj.transform.localScale = new Vector3(start.x + speed.x * curtime, start.y + speed.y * curtime, start.z + speed.z * curtime);
            curtime += Time.deltaTime;
            yield return null;
        }
        obj.transform.localScale = scale;
    }

    /// <summary>
    /// 오브젝트의 rotation.z의 값을 targetAngle까지 바꾸기위해 증감해야될 z값을 반환한다. 
    /// 사용 방법 : RotationTo(time, new Vector3(0f, 0f, RotationAngle(gameObject, angle)), index);
    /// </summary>
    public static float RotationAngle(GameObject obj, float targetAngle)
    {
        if (obj.transform.eulerAngles.z > 180.0f)
            return (targetAngle - (obj.transform.eulerAngles.z - 360.0f));
        else
            return (targetAngle - obj.transform.eulerAngles.z);
    }

    /// <summary>
    /// time 시간동안 obj를 angle만큼 회전한다.
    /// </summary>
    public static IEnumerator RotationToC(float time, Vector3 angle, GameObject obj)
    {
        Vector3 speed = new Vector3(angle.x / time, angle.y / time, angle.z / time);
        Vector3 start = new Vector3(obj.transform.eulerAngles.x, obj.transform.eulerAngles.y, obj.transform.eulerAngles.z);
        Quaternion end = Quaternion.Euler(obj.transform.eulerAngles.x + angle.x,
            obj.transform.eulerAngles.y + angle.y, obj.transform.eulerAngles.z + angle.z);

        float curtime = 0.0f;
        while (curtime < time)
        {
            obj.transform.eulerAngles = new Vector3(start.x + speed.x * curtime, start.y + speed.y * curtime, start.z + speed.z * curtime);
            curtime += Time.deltaTime;
            yield return null;
        }
        obj.transform.localRotation = end;
    }

    /// <summary>
    /// time 시간동안 obj의 위치를 start에서 end로 이동시킨다.
    /// </summary>
    public static IEnumerator MoveObjC(float time, Vector3 start, Vector3 end, GameObject obj)
    {
        Vector3 speed = new Vector3(end.x - start.x, end.y - start.y, end.z - start.z) / time;

        float curTime = 0f;
        while (curTime < time)
        {
            obj.transform.localPosition = new Vector3(start.x + speed.x * curTime, start.y + speed.y * curTime, start.z + speed.z * curTime);            
            curTime += Time.deltaTime;
            yield return null;
        }
        obj.transform.localPosition = end;
    }

    /// <summary>
    /// time 시간동안 obj의 위치를 베지어 곡선을 통해 이동시킨다.
    /// </summary>
    /// <param name="start">시작점</param>
    /// <param name="p1">경유점 1</param>
    /// <param name="p2">경유점 2</param>
    /// <param name="end">종점</param>
    /// <returns></returns>
    public static IEnumerator CurveMoveObjC(float time, Vector3 start, Vector3 p1, Vector3 p2, Vector3 end, GameObject obj)
    {
        float speed = 1f / time;
        float curTime = 0f;
        float moveCurveRate = 0f;
        while (curTime < time)
        {
            moveCurveRate = speed * curTime;
            obj.transform.localPosition = Curve.BezierCurve(moveCurveRate, start, p1, p2, end);
            curTime += Time.deltaTime;
            yield return null;
        }
        obj.transform.localPosition = end;
    }
}
