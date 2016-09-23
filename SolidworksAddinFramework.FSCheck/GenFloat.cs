using FsCheck;

namespace WeinCadSW.Spec.FsCheck
{
    /// <summary>
    /// Different float generators
    /// </summary>
    public static class GenFloat
    {
        public static Gen<double> Normal =>
            Arb.Default.NormalFloat().Generator.Select(f=>f.Item);

    }
}