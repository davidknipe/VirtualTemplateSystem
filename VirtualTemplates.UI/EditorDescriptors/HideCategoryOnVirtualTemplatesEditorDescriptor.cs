using System;
using System.Collections.Generic;
using EPiServer.Cms.Shell.UI.ObjectEditing;
using EPiServer.Core;
using EPiServer.Shell.ObjectEditing;
using EPiServer.Shell.ObjectEditing.EditorDescriptors;
using VirtualTemplates.Core.Models;

namespace VirtualTemplates.UI.EditorDescriptors
{
    [EditorDescriptorRegistration(TargetType = typeof(CategoryList),
        EditorDescriptorBehavior = EditorDescriptorBehavior.Default)]
    public class HideCategoryOnVirtualTemplatesEditorDescriptor : EditorDescriptor
    {
        public override void ModifyMetadata(
            ExtendedMetadata metadata,
            IEnumerable<Attribute> attributes)
        {
            base.ModifyMetadata(metadata, attributes);

            var contentMetadata = (ContentDataMetadata)metadata;
            var ownerContent = contentMetadata.OwnerContent;
            if (ownerContent is VirtualTemplateContentBase
                && metadata.PropertyName == "icategorizable_category")
            {
                metadata.ShowForEdit = false;
            }
        }
    }

}
