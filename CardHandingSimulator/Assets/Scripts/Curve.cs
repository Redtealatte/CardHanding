using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Curve : MonoBehaviour
{
    // p0 시작점, p1 중간점1, p2 중간점2, p3 종료점
    public static Vector3 BezierCurve(float rate, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        // 전체 지점의 위치 정보를 저장하기 위한 위치리스트(배열)
        List<Vector3> t_pointList = new List<Vector3>();

        // 시작지점 위치값을 위치리스트 에 추가
        t_pointList.Add(p0);

        // 중간지점 위치값을 위치리스트 에 추가
        t_pointList.Add(p1);
        t_pointList.Add(p2);

        // 종료지점 위치값을 위치리스트에 추가
        t_pointList.Add(p3);

        // 오브젝트의 위치를 계산하기 위한 계산 시작
        // 위치가 1개 남을때 까지 반복문 으로 계산
        while (t_pointList.Count > 1)
        {
            //선형보간으로 계산된 결과값을 저장하기 위한 결과값리스트(배열)
            List<Vector3> t_resultList = new List<Vector3>();

            for (int i = 0; i < t_pointList.Count - 1; i++)
            {
                // 현재 지점과 다음 지점의 선형보간을 통한 위치값 계산
                Vector3 result = Vector3.Lerp(t_pointList[i], t_pointList[i + 1], rate);

                // 계산된 위치 값을 결과값리스트에 대입
                t_resultList.Add(result);
            }
            // 위치리스트를 결과값 리스트로 변경
            t_pointList = t_resultList;
        }
        return t_pointList[0];
    }
}
