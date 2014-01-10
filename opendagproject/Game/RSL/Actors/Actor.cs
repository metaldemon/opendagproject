using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pencil.Gaming;
using Pencil.Gaming.MathUtils;
using opendagproject.Game.Input;
using opendagproject.Game.Graphics;
using opendagproject.Content;

namespace opendagproject.Game.RSL.Actors
{
    public class Actor
    {
        public string name = string.Empty;
        public string baseActorName = string.Empty;
        public List<Variable> variableList = new List<Variable>();
        public Sprite sprite;

        public void setVariable(string name, object obj)
        {
            variableList.First(x => x.name == name).setValue(obj);
        }
        
        public void actorUpdate()
        {
            if (this.sprite != null)
            {
                this.sprite.position = new Vector2(float.Parse(this.getVariable("positionX").ToString()), float.Parse(this.getVariable("positionY").ToString()));
                if (InputManager.currentKeyState.mouseState.RightButton && !InputManager.previousKeyState.mouseState.RightButton)
                {
                    Vector2 mouseposition = new Vector2(InputManager.currentKeyState.mouseState.X, InputManager.currentKeyState.mouseState.Y) -
                        Graphics.Graphics.cameraPosition - new Vector2(GameUtils.resolutionX / 2, GameUtils.resolutionY / 2);
                    if (GameUtils.getDistance(mouseposition, this.sprite.position) < ((float)this.sprite.width + (float)this.sprite.height) / 2f)
                    {
                        onClick();
                    }
                }
            }
        }

        public void tick()
        {
            this.name = (string)getVariable("name");
        }

        public virtual void onClick()
        {
            
        }

        public virtual void onRemove()
        {

        }

        public object getVariable(string name)
        {
            try
            {
                return variableList.First(x => x.name == name).getValue();
            }
            catch (Exception e)
            {
                ExceptionHandler.printException("RSL Actor error: Cannot find Actor variable: \"" + name + "\"", ConsoleColor.DarkRed, ExceptionHandler.ExceptionHandle.CLOSEONKEY);
            }
            return false;
        }
    }
}
