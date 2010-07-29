﻿using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

using Composite.WebClient.UiControlLib.Foundation;
using Composite.ResourceSystem;

namespace Composite.WebClient.UiControlLib
{
    /// <exclude />
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)] 
    public class ClickButton : LinkButton
    {
        public ClickButton()
        {
            this.AutoPostBack = true;
        }

        [Category("Appearance"), DefaultValue(""), Description("The id the ui client should see")]
        public virtual string CustomClientId { get; set; }

        [Category("Appearance"), DefaultValue(""), Description("Client sceipt that ensure post back should be appended to CustomClientScript. Default is true.")]
        public virtual bool AutoPostBack { get; set; }
        
        [Category("Appearance"), DefaultValue(""), Description("Image to show in the buttom")]
        public virtual string ImageUrl { get; set; }

        [Category("Appearance"), DefaultValue(""), Description("Image to show in the buttom when the button is disabled")]
        public virtual string ImageUrlWhenDisabled { get; set; }

        protected override void Render(HtmlTextWriter writer)
        {
            writer.WriteBeginTag("ui:clickbutton");

            writer.WriteAttribute("label", StringResourceSystemFacade.ParseString(this.Text));
            writer.WriteAttribute("callbackid", this.ClientID);

            string oncommand = "";
            if (string.IsNullOrEmpty(this.OnClientClick) == false)
            {
                oncommand += this.OnClientClick;
            }
            if (this.AutoPostBack == true)
            {
                if (oncommand.Length > 0 && oncommand.Trim().EndsWith(";") == false)
                {
                    oncommand += ";";
                }
				
				// now implied by callbackid!
                // oncommand += "this.dispatchAction(PageBinding.ACTION_DOPOSTBACK)";
            }
            if (string.IsNullOrEmpty(oncommand) == false)
            {
                writer.WriteAttribute("oncommand", oncommand);
            }

            if (string.IsNullOrEmpty(this.CustomClientId) == false)
            {
                writer.WriteAttribute("id", this.CustomClientId);
            }
            if (string.IsNullOrEmpty(this.ImageUrl) == false)
            {
                writer.WriteAttribute("image", this.ImageUrl);
            }
            if (string.IsNullOrEmpty(this.ImageUrlWhenDisabled) == false)
            {
                writer.WriteAttribute("image-disabled", this.ImageUrlWhenDisabled);
            }
            if (this.Enabled == false)
            {
                writer.WriteAttribute("isdisabled", "true");
            }

            this.WriteClientAttributes(writer);


            writer.Write(HtmlTextWriter.SelfClosingTagEnd);
        }
    }
}