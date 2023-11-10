namespace GameProto
{
    public partial class Input
    {
        public bool Equals(Input tInput)
        {
            if (tInput == null)
            {
                return false;
            }

            if (this.InputV != tInput.InputV || this.InputH != tInput.InputH 
                || this.MousePosX != tInput.MousePosX || this.MousePosY != tInput.MousePosY
                || this.IsFire != tInput.IsFire || this.IsSpeedUp != tInput.IsSpeedUp)
            {
                return false;
            }

            return true;
        }
    }

    public partial class InputFrame
    {
        public bool Equals(InputFrame tInputFrame)
        {
            if (tInputFrame == null)
            {
                return false;
            }

            if (this.Tick != tInputFrame.Tick)
            {
                return false;
            }

            if (this.LocalId != tInputFrame.LocalId)
            {
                return false;
            }

            if (this.Input != null && tInputFrame.Input != null && !this.Input.Equals(tInputFrame.Input))
            {
                return false;
            }

            if ((this.Input == null || tInputFrame.Input == null) && this.Input != tInputFrame.Input)
            {
                return false;
            }

            return true;
        }
    }

    public partial class ServerFrame
    {
        public bool Equals(ServerFrame tFrame)
        {
            if (tFrame == null)
            {
                return false;
            }

            if (Tick != tFrame.Tick)
            {
                return false;
            }

            if (InputFrames.Count != tFrame.InputFrames.Count)
            {
                return false;
            }

            for (int i = 0; i < InputFrames.Count; i++)
            {
                InputFrame inputFrame1 = InputFrames[i];
                InputFrame inputFrame2 = tFrame.InputFrames[i];

                if (!inputFrame1.Equals(inputFrame2))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
