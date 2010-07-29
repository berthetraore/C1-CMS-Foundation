using System;
using System.Collections.Generic;
using System.Linq;
using Composite.Elements;
using Composite.Security;
using Composite.Types;


namespace Composite.WebClient.Services.TreeServiceObjects.ExtensionMethods
{
    /// <exclude />
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)] 
    public static class ElementExtensionMethods
    {
        public static ClientElement GetClientElement(this Element element)
        {
            if (element.VisualData.Icon == null || element.Actions.Where(a => a.VisualData.Icon == null).Any())
            {
                throw new InvalidOperationException(string.Format("Unable to create ClientElement from Element with entity token '{0}'. The element or one of its actions is missing an icon definition.", element.ElementHandle.EntityToken.Serialize()));
            }

            string entityToken = EntityTokenSerializer.Serialize(element.ElementHandle.EntityToken, true);

            string piggyBag = element.ElementHandle.SerializedPiggyback;
            
            ClientElement clientElement = new ClientElement
                   {
                       ElementKey = string.Format("{0}{1}{2}", element.ElementHandle.ProviderName, entityToken, piggyBag), 
                       ProviderName = element.ElementHandle.ProviderName,
                       EntityToken = entityToken,
                       Piggybag = piggyBag,
                       PiggybagHash = HashSigner.GetSignedHash(piggyBag).Serialize(),
                       Label = element.VisualData.Label,
                       HasChildren = element.VisualData.HasChildren,
                       IsDisabled = element.VisualData.IsDisabled,
                       Icon = element.VisualData.Icon,
                       OpenedIcon = element.VisualData.OpenedIcon,
                       ToolTip = element.VisualData.ToolTip,
                       Actions = element.Actions.ToClientActionList(),
                       PropertyBag = element.PropertyBag.ToClientPropertyBag(),
                       TagValue = element.TagValue,
                       ContainsTaggedActions = element.Actions.Where(f => f.TagValue != null).Any(),
                       TreeLockEnabled = element.TreeLockBehavior == TreeLockBehavior.Normal
                   };

            clientElement.ActionKeys =
                (from clientAction in clientElement.Actions
                 select clientAction.ActionKey).ToList();

            if (element.MovabilityInfo.DragType != null) clientElement.DragType = element.MovabilityInfo.GetHashedTypeIdentifier();

            List<string> apoptables = element.MovabilityInfo.GetDropHashTypeIdentifiers();
            if (apoptables != null && apoptables.Count > 0)
            {
                clientElement.DropTypeAccept = apoptables;
            }

            clientElement.DetailedDropSupported = element.MovabilityInfo.SupportsIndexedPosition;

            return clientElement;
        }



        public static List<ClientElement> ToClientElementList(this List<Element> elements)
        {
            List<ClientElement> list = new List<ClientElement>();

            foreach (Element element in elements)
            {
                list.Add(element.GetClientElement());
            }

            return list;
        }



        public static List<KeyValuePair> ToClientPropertyBag(this Dictionary<string, string> propertyBag)
        {
            if (propertyBag == null || propertyBag.Count == 0) return null;

            List<KeyValuePair> result = new List<KeyValuePair>();

            foreach (KeyValuePair<string, string> kvp in propertyBag)
            {
                result.Add(new KeyValuePair(kvp.Key, kvp.Value));
            }

            return result;
        }

    }
}
