namespace BaseFramework
{
    /// <summary>
    /// 定义一个委托，该委托没有参数，没有返回值。
    /// </summary>
    public delegate void BaseFrameworkAction();

    /// <summary>
    /// 定义一个委托，该委托有1个参数，没有返回值。
    /// </summary>
    /// <typeparam name="T">参数类型。</typeparam>
    /// <param name="arg">参数。</param>
    public delegate void BaseFrameworkAction<in T>(T arg);

    /// <summary>
    /// 定义一个委托，该委托有2个参数，没有返回值。
    /// </summary>
    /// <typeparam name="T1">参数1类型。</typeparam>
    /// <typeparam name="T2">参数2类型。</typeparam>
    /// <param name="arg1">参数1。</param>
    /// <param name="arg2">参数2。</param>
    public delegate void BaseFrameworkAction<in T1, in T2>(T1 arg1, T2 arg2);

    /// <summary>
    /// 定义一个委托，该委托有3个个参数，没有返回值。
    /// </summary>
    /// <typeparam name="T1">参数1类型。</typeparam>
    /// <typeparam name="T2">参数2类型。</typeparam>
    /// <typeparam name="T3">参数3类型。</typeparam>
    /// <param name="arg1">参数1。</param>
    /// <param name="arg2">参数2。</param>
    /// <param name="arg3">参数3。</param>
    public delegate void BaseFrameworkAction<in T1, in T2, in T3>(T1 arg1, T2 arg2, T3 arg3);

    /// <summary>
    /// 定义一个委托，该委托有4个参数，没有返回值。
    /// </summary>
    /// <typeparam name="T1">参数1类型。</typeparam>
    /// <typeparam name="T2">参数2类型。</typeparam>
    /// <typeparam name="T3">参数3类型。</typeparam>
    /// <typeparam name="T4">参数4类型。</typeparam>
    /// <param name="arg1">参数1。</param>
    /// <param name="arg2">参数2。</param>
    /// <param name="arg3">参数3。</param>
    /// <param name="arg4">参数4。</param>
    public delegate void BaseFrameworkAction<in T1, in T2, in T3, in T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);

    /// <summary>
    /// 定义一个委托，该委托有5个参数，没有返回值。
    /// </summary>
    /// <typeparam name="T1">参数类型1。</typeparam>
    /// <typeparam name="T2">参数类型2。</typeparam>
    /// <typeparam name="T3">参数类型3。</typeparam>
    /// <typeparam name="T4">参数类型4。</typeparam>
    /// <typeparam name="T5">参数类型5。</typeparam>
    /// <param name="arg1">参数1。</param>
    /// <param name="arg2">参数2。</param>
    /// <param name="arg3">参数3。</param>
    /// <param name="arg4">参数4。</param>
    /// <param name="arg5">参数5。</param>
    public delegate void BaseFrameworkAction<in T1, in T2, in T3, in T4, in T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);

    /// <summary>
    /// 定义一个委托，该委托有6个参数，没有返回值。
    /// </summary>
    /// <typeparam name="T1">参数类型1。</typeparam>
    /// <typeparam name="T2">参数类型2。</typeparam>
    /// <typeparam name="T3">参数类型3。</typeparam>
    /// <typeparam name="T4">参数类型4。</typeparam>
    /// <typeparam name="T5">参数类型5。</typeparam>
    /// <typeparam name="T6">参数类型6。</typeparam>
    /// <param name="arg1">参数1。</param>
    /// <param name="arg2">参数2。</param>
    /// <param name="arg3">参数3。</param>
    /// <param name="arg4">参数4。</param>
    /// <param name="arg5">参数5。</param>
    /// <param name="arg6">参数6。</param>
    public delegate void BaseFrameworkAction<in T1, in T2, in T3, in T4, in T5, in T6>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);

    /// <summary>
    /// 定义一个委托，该委托有7个参数，没有返回值。
    /// </summary>
    /// <typeparam name="T1">参数类型1。</typeparam>
    /// <typeparam name="T2">参数类型2。</typeparam>
    /// <typeparam name="T3">参数类型3。</typeparam>
    /// <typeparam name="T4">参数类型4。</typeparam>
    /// <typeparam name="T5">参数类型5。</typeparam>
    /// <typeparam name="T6">参数类型6。</typeparam>
    /// <typeparam name="T7">参数类型7。</typeparam>
    /// <param name="arg1">参数1。</param>
    /// <param name="arg2">参数2。</param>
    /// <param name="arg3">参数3。</param>
    /// <param name="arg4">参数4。</param>
    /// <param name="arg5">参数5。</param>
    /// <param name="arg6">参数6。</param>
    /// <param name="arg7">参数7。</param>
    public delegate void BaseFrameworkAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);

    /// <summary>
    /// 定义一个委托，该委托有8个参数，没有返回值。
    /// </summary>
    /// <typeparam name="T1">参数1类型。</typeparam>
    /// <typeparam name="T2">参数2类型。</typeparam>
    /// <typeparam name="T3">参数3类型。</typeparam>
    /// <typeparam name="T4">参数4类型。</typeparam>
    /// <typeparam name="T5">参数5类型。</typeparam>
    /// <typeparam name="T6">参数6类型。</typeparam>
    /// <typeparam name="T7">参数7类型。</typeparam>
    /// <typeparam name="T8">参数8类型。</typeparam>
    /// <param name="arg1">参数1。</param>
    /// <param name="arg2">参数2。</param>
    /// <param name="arg3">参数3。</param>
    /// <param name="arg4">参数4。</param>
    /// <param name="arg5">参数5。</param>
    /// <param name="arg6">参数6。</param>
    /// <param name="arg7">参数7。</param>
    /// <param name="arg8">参数8。</param>
    public delegate void BaseFrameworkAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);

    /// <summary>
    /// 定义一个委托，该委托有9个参数，没有返回值。
    /// </summary>
    /// <typeparam name="T1">参数1类型。</typeparam>
    /// <typeparam name="T2">参数2类型。</typeparam>
    /// <typeparam name="T3">参数3类型。</typeparam>
    /// <typeparam name="T4">参数4类型。</typeparam>
    /// <typeparam name="T5">参数5类型。</typeparam>
    /// <typeparam name="T6">参数6类型。</typeparam>
    /// <typeparam name="T7">参数7类型。</typeparam>
    /// <typeparam name="T8">参数8类型。</typeparam>
    /// <typeparam name="T9">参数9类型。</typeparam>
    /// <param name="arg1">参数1。</param>
    /// <param name="arg2">参数2。</param>
    /// <param name="arg3">参数3。</param>
    /// <param name="arg4">参数4。</param>
    /// <param name="arg5">参数5。</param>
    /// <param name="arg6">参数6。</param>
    /// <param name="arg7">参数7。</param>
    /// <param name="arg8">参数8。</param>
    /// <param name="arg9">参数9。</param>
    public delegate void BaseFrameworkAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9);

    /// <summary>
    /// 定义一个委托，该委托有10个参数，没有返回值。
    /// </summary>
    /// <typeparam name="T1">参数1类型。</typeparam>
    /// <typeparam name="T2">参数2类型。</typeparam>
    /// <typeparam name="T3">参数3类型。</typeparam>
    /// <typeparam name="T4">参数4类型。</typeparam>
    /// <typeparam name="T5">参数5类型。</typeparam>
    /// <typeparam name="T6">参数6类型。</typeparam>
    /// <typeparam name="T7">参数7类型。</typeparam>
    /// <typeparam name="T8">参数8类型。</typeparam>
    /// <typeparam name="T9">参数9类型。</typeparam>
    /// <typeparam name="T10">参数10类型。</typeparam>
    /// <param name="arg1">参数1。</param>
    /// <param name="arg2">参数2。</param>
    /// <param name="arg3">参数3。</param>
    /// <param name="arg4">参数4。</param>
    /// <param name="arg5">参数5。</param>
    /// <param name="arg6">参数6。</param>
    /// <param name="arg7">参数7。</param>
    /// <param name="arg8">参数8。</param>
    /// <param name="arg9">参数9。</param>
    /// <param name="arg10">参数10。</param>
    public delegate void BaseFrameworkAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10);

    /// <summary>
    /// 定义一个委托，该委托有11个参数，没有返回值。
    /// </summary>
    /// <typeparam name="T1">参数1类型。</typeparam>
    /// <typeparam name="T2">参数2类型。</typeparam>
    /// <typeparam name="T3">参数3类型。</typeparam>
    /// <typeparam name="T4">参数4类型。</typeparam>
    /// <typeparam name="T5">参数5类型。</typeparam>
    /// <typeparam name="T6">参数6类型。</typeparam>
    /// <typeparam name="T7">参数7类型。</typeparam>
    /// <typeparam name="T8">参数8类型。</typeparam>
    /// <typeparam name="T9">参数9类型。</typeparam>
    /// <typeparam name="T10">参数10类型。</typeparam>
    /// <typeparam name="T11">参数11类型。</typeparam>
    /// <param name="arg1">参数1。</param>
    /// <param name="arg2">参数2。</param>
    /// <param name="arg3">参数3。</param>
    /// <param name="arg4">参数4。</param>
    /// <param name="arg5">参数5。</param>
    /// <param name="arg6">参数6。</param>
    /// <param name="arg7">参数7。</param>
    /// <param name="arg8">参数8。</param>
    /// <param name="arg9">参数9。</param>
    /// <param name="arg10">参数10。</param>
    /// <param name="arg11">参数11。</param>
    public delegate void BaseFrameworkAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11);

    /// <summary>
    /// 定义一个委托，该委托有12个参数，没有返回值。
    /// </summary>
    /// <typeparam name="T1">参数1类型。</typeparam>
    /// <typeparam name="T2">参数2类型。</typeparam>
    /// <typeparam name="T3">参数3类型。</typeparam>
    /// <typeparam name="T4">参数4类型。</typeparam>
    /// <typeparam name="T5">参数5类型。</typeparam>
    /// <typeparam name="T6">参数6类型。</typeparam>
    /// <typeparam name="T7">参数7类型。</typeparam>
    /// <typeparam name="T8">参数8类型。</typeparam>
    /// <typeparam name="T9">参数9类型。</typeparam>
    /// <typeparam name="T10">参数10类型。</typeparam>
    /// <typeparam name="T11">参数11类型。</typeparam>
    /// <typeparam name="T12">参数12类型。</typeparam>
    /// <param name="arg1">参数1。</param>
    /// <param name="arg2">参数2。</param>
    /// <param name="arg3">参数3。</param>
    /// <param name="arg4">参数4。</param>
    /// <param name="arg5">参数5。</param>
    /// <param name="arg6">参数6。</param>
    /// <param name="arg7">参数7。</param>
    /// <param name="arg8">参数8。</param>
    /// <param name="arg9">参数9。</param>
    /// <param name="arg10">参数10。</param>
    /// <param name="arg11">参数11。</param>
    /// <param name="arg12">参数12。</param>
    public delegate void BaseFrameworkAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12);

    /// <summary>
    /// 定义一个委托，该委托有13个参数，没有返回值。
    /// </summary>
    /// <typeparam name="T1">参数1类型。</typeparam>
    /// <typeparam name="T2">参数2类型。</typeparam>
    /// <typeparam name="T3">参数3类型。</typeparam>
    /// <typeparam name="T4">参数4类型。</typeparam>
    /// <typeparam name="T5">参数5类型。</typeparam>
    /// <typeparam name="T6">参数6类型。</typeparam>
    /// <typeparam name="T7">参数7类型。</typeparam>
    /// <typeparam name="T8">参数8类型。</typeparam>
    /// <typeparam name="T9">参数9类型。</typeparam>
    /// <typeparam name="T10">参数10类型。</typeparam>
    /// <typeparam name="T11">参数11类型。</typeparam>
    /// <typeparam name="T12">参数12类型。</typeparam>
    /// <typeparam name="T13">参数13类型。</typeparam>
    /// <param name="arg1">参数1。</param>
    /// <param name="arg2">参数2。</param>
    /// <param name="arg3">参数3。</param>
    /// <param name="arg4">参数4。</param>
    /// <param name="arg5">参数5。</param>
    /// <param name="arg6">参数6。</param>
    /// <param name="arg7">参数7。</param>
    /// <param name="arg8">参数8。</param>
    /// <param name="arg9">参数9。</param>
    /// <param name="arg10">参数10。</param>
    /// <param name="arg11">参数11。</param>
    /// <param name="arg12">参数12。</param>
    /// <param name="arg13">参数13。</param>
    public delegate void BaseFrameworkAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13);

    /// <summary>
    /// 定义一个委托，该委托有14个参数，没有返回值。
    /// </summary>
    /// <typeparam name="T1">参数1类型。</typeparam>
    /// <typeparam name="T2">参数2类型。</typeparam>
    /// <typeparam name="T3">参数3类型。</typeparam>
    /// <typeparam name="T4">参数4类型。</typeparam>
    /// <typeparam name="T5">参数5类型。</typeparam>
    /// <typeparam name="T6">参数6类型。</typeparam>
    /// <typeparam name="T7">参数7类型。</typeparam>
    /// <typeparam name="T8">参数8类型。</typeparam>
    /// <typeparam name="T9">参数9类型。</typeparam>
    /// <typeparam name="T10">参数10类型。</typeparam>
    /// <typeparam name="T11">参数11类型。</typeparam>
    /// <typeparam name="T12">参数12类型。</typeparam>
    /// <typeparam name="T13">参数13类型。</typeparam>
    /// <typeparam name="T14">参数14类型。</typeparam>
    /// <param name="arg1">参数1。</param>
    /// <param name="arg2">参数2。</param>
    /// <param name="arg3">参数3。</param>
    /// <param name="arg4">参数4。</param>
    /// <param name="arg5">参数5。</param>
    /// <param name="arg6">参数6。</param>
    /// <param name="arg7">参数7。</param>
    /// <param name="arg8">参数8。</param>
    /// <param name="arg9">参数9。</param>
    /// <param name="arg10">参数10。</param>
    /// <param name="arg11">参数11。</param>
    /// <param name="arg12">参数12。</param>
    /// <param name="arg13">参数13。</param>
    /// <param name="arg14">参数14。</param>
    public delegate void BaseFrameworkAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14);

    /// <summary>
    /// 定义一个委托，该委托有15个参数，没有返回值。
    /// </summary>
    /// <typeparam name="T1">参数1类型。</typeparam>
    /// <typeparam name="T2">参数2类型。</typeparam>
    /// <typeparam name="T3">参数3类型。</typeparam>
    /// <typeparam name="T4">参数4类型。</typeparam>
    /// <typeparam name="T5">参数5类型。</typeparam>
    /// <typeparam name="T6">参数6类型。</typeparam>
    /// <typeparam name="T7">参数7类型。</typeparam>
    /// <typeparam name="T8">参数8类型。</typeparam>
    /// <typeparam name="T9">参数9类型。</typeparam>
    /// <typeparam name="T10">参数10类型。</typeparam>
    /// <typeparam name="T11">参数11类型。</typeparam>
    /// <typeparam name="T12">参数12类型。</typeparam>
    /// <typeparam name="T13">参数13类型。</typeparam>
    /// <typeparam name="T14">参数14类型。</typeparam>
    /// <typeparam name="T15">参数15类型。</typeparam>
    /// <param name="arg1">参数1。</param>
    /// <param name="arg2">参数2。</param>
    /// <param name="arg3">参数3。</param>
    /// <param name="arg4">参数4。</param>
    /// <param name="arg5">参数5。</param>
    /// <param name="arg6">参数6。</param>
    /// <param name="arg7">参数7。</param>
    /// <param name="arg8">参数8。</param>
    /// <param name="arg9">参数9。</param>
    /// <param name="arg10">参数10。</param>
    /// <param name="arg11">参数11。</param>
    /// <param name="arg12">参数12。</param>
    /// <param name="arg13">参数13。</param>
    /// <param name="arg14">参数14。</param>
    /// <param name="arg15">参数15。</param>
    public delegate void BaseFrameworkAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14, in T15>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15);

    /// <summary>
    /// 定义一个委托，该委托有16个参数，没有返回值。
    /// </summary>
    /// <typeparam name="T1">参数1类型。</typeparam>
    /// <typeparam name="T2">参数2类型。</typeparam>
    /// <typeparam name="T3">参数3类型。</typeparam>
    /// <typeparam name="T4">参数4类型。</typeparam>
    /// <typeparam name="T5">参数5类型。</typeparam>
    /// <typeparam name="T6">参数6类型。</typeparam>
    /// <typeparam name="T7">参数7类型。</typeparam>
    /// <typeparam name="T8">参数8类型。</typeparam>
    /// <typeparam name="T9">参数9类型。</typeparam>
    /// <typeparam name="T10">参数10类型。</typeparam>
    /// <typeparam name="T11">参数11类型。</typeparam>
    /// <typeparam name="T12">参数12类型。</typeparam>
    /// <typeparam name="T13">参数13类型。</typeparam>
    /// <typeparam name="T14">参数14类型。</typeparam>
    /// <typeparam name="T15">参数15类型。</typeparam>
    /// <typeparam name="T16">参数16类型。</typeparam>
    /// <param name="arg1">参数1。</param>
    /// <param name="arg2">参数2。</param>
    /// <param name="arg3">参数3。</param>
    /// <param name="arg4">参数4。</param>
    /// <param name="arg5">参数5。</param>
    /// <param name="arg6">参数6。</param>
    /// <param name="arg7">参数7。</param>
    /// <param name="arg8">参数8。</param>
    /// <param name="arg9">参数9。</param>
    /// <param name="arg10">参数10。</param>
    /// <param name="arg11">参数11。</param>
    /// <param name="arg12">参数12。</param>
    /// <param name="arg13">参数13。</param>
    /// <param name="arg14">参数14。</param>
    /// <param name="arg15">参数15。</param>
    /// <param name="arg16">参数16。</param>
    public delegate void BaseFrameworkAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14, in T15, in T16>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16);
}
