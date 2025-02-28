Shader "Custom/Hood"
{
    SubShader
    {
        Tags { "Queue" = "Overlay" }  // ����� ���������� ������� ��� ���������.
        Pass
        {
            Stencil
            {
                Ref 1              // ��������� �������, ���������� ��������� 1.
                Comp NotEqual      // ������������ ������ �� �������, ������� �� ��������� ������ �����.
                Pass Keep          // �� ������ ����������� �����.
            }
            // ����������� ������ ��������.
            ZWrite On
            ZTest LEqual
            Blend SrcAlpha OneMinusSrcAlpha
        }
    }
}
