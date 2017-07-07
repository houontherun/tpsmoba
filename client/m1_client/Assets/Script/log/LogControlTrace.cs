/*******************************************************************
** ��  Ȩ:	���������������޹�˾
** ������:	�ų���
** ��  ��:	2014/12/20 15:56
** ��  ��:	1.0
** ��  ��:	LOG���
** Ӧ  ��:  	��Ҫ���ڴ�ӡ��Ϸ�������LOG����Ļ�����ǵ��ļ�	
********************************************************************/
using UnityEngine;
/// <summary>
/// LOG�������̨���༭��ר��
/// </summary>
public class LogControlTrace : LogTrace
{
    public LogControlTrace()
    {

    }

    /// <summary>
    /// ��ȡ����ID
    /// </summary>
    public override LogTraceType Type { get { return LogTraceType.Control; } }

    /// <summary>
    /// �����־
    /// </summary>
    public override void AddLog(LogTraceNode node)
    {
        switch ((LogType)node.type)
        {
            case LogType.Log:
                Debug.Log(node.msg);
                break;
            case LogType.Warning:
                Debug.LogWarning(node.msg);
                break;
            case LogType.Error:
                Debug.LogError(node.msg);
                break;
        }
    }
}