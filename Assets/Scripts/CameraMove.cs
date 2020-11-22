using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    Vector3 startPos;
    Quaternion startRot;

    Vector3 destination;
    Quaternion destRot;

    public bool trackingTarget;

    public float rotateSpeed;

    public float wheelSpeed;

    public float maxwheelDist;

    public float minwheelDist;

    float wheelDist=0;

    float angle;

    public float maxRotAngle;
    public float minRotAngle;

    void Start()
    {
        startPos = transform.position;
        startRot = transform.rotation;

    }

    // Update is called once per frame
    void Update()
    {
        float _mouseX = Input.GetAxis("Mouse X");
        float _mouseY = Input.GetAxis("Mouse Y");
        CameraRotate(_mouseX, -_mouseY);
        CameraFOV();
    }

    public void setDestination(Vector3 dest, Quaternion quat)
    {
        destination = dest;
        destRot = quat;
    }

    public void setDestination(Vector3 dest)
    {
        destination = dest;
    }

    public void setDestination(Quaternion quat)
    {
        destRot = quat;
    }

    public void cameraMoveTo()
    {
        if (trackingTarget)
        {
            transform.position = transform.position + (destination - transform.position) * 0.1f;
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + (destRot.eulerAngles - transform.rotation.eulerAngles) * 0.1f);
        }
    }

    public void CameraRotate(float _mouseX, float _mouseY)
    {
        //注意!!! 此处是 GetMouseButton() 表示一直长按鼠标左键；不是 GetMouseButtonDown()
        if (Input.GetMouseButton(0))
        {
            //控制相机绕中心点(centerPoint)水平旋转
            transform.RotateAround(Vector3.zero, Vector3.up, _mouseX * rotateSpeed);

            //记录相机绕中心点垂直旋转的总角度
            angle += _mouseY * rotateSpeed;

            //如果总角度超出指定范围，结束这一帧（！用于解决相机旋转到模型底部的Bug！）
            //（这样做其实还有小小的Bug，能发现的网友麻烦留言告知解决办法或其他更好的方法）
            if (angle > maxRotAngle || angle < minRotAngle)
            {
                return;
            }

            //控制相机绕中心点垂直旋转(！注意此处的旋转轴时相机自身的x轴正方向！)
            transform.RotateAround(Vector3.zero, transform.right, _mouseY * rotateSpeed);
        }
    }

    public void CameraFOV()
    {
        //获取鼠标滚轮的滑动量
        float wheel = Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * wheelSpeed;

        wheelDist += wheel;

        if (wheelDist > maxwheelDist || wheelDist < minwheelDist) return;

        //改变相机的位置
        transform.Translate(Vector3.forward * wheel);
    }
}
