/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Utilities.ClothProp
{
    public class ClothPropData
    {
        public byte Component;
        public short Drawable;
        public short Texture;

        public ClothPropData(byte component, short drawable, short texture)
        {
            this.Component = component;
            this.Drawable = drawable;
            this.Texture = texture;
        }
    }
}
