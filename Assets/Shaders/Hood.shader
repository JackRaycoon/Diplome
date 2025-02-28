Shader "Custom/Hood"
{
    SubShader
    {
        Tags { "Queue" = "Overlay" }  // Задаём правильный порядок для отрисовки.
        Pass
        {
            Stencil
            {
                Ref 1              // Проверяем пиксели, помеченные значением 1.
                Comp NotEqual      // Отрисовываем только те пиксели, которые не находятся внутри маски.
                Pass Keep          // Не меняем стенсильный буфер.
            }
            // Стандартный рендер капюшона.
            ZWrite On
            ZTest LEqual
            Blend SrcAlpha OneMinusSrcAlpha
        }
    }
}
