using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Options;

namespace ViewComponentTagHelper
{
    public class ReferenceManager
    {
        private List<MetadataReference> _metadataReferences;
        private readonly ApplicationPartManager _partManager;
        private readonly IList<MetadataReference> _additionalMetadataReferences;

        public ReferenceManager(
            IOptions<RazorViewEngineOptions> optionsAccessor,
            ApplicationPartManager partManager
            )
        {
            _additionalMetadataReferences = optionsAccessor.Value.AdditionalCompilationReferences;
            _metadataReferences = new List<MetadataReference>();
            _partManager = partManager;
        }

        public List<MetadataReference> GetReferences()
        {
            AddReferences(GetDefaultReferences());
            return _metadataReferences;
        }

        public void AddReference(MetadataReference metadataReference)
        {
            _metadataReferences.Add(metadataReference);
        }

        public void AddReferences(IList<MetadataReference> metadataReferences)
        {
            _metadataReferences = _metadataReferences.Concat(metadataReferences).Distinct().ToList();
        }

        private IList<MetadataReference> GetDefaultReferences()
        {
            var feature = new MetadataReferenceFeature();
            _partManager.PopulateFeature(feature);
            var applicationReferences = feature.MetadataReferences;

            if (_additionalMetadataReferences.Count == 0)
            {
                return applicationReferences;
            }

            var compilationReferences = new List<MetadataReference>(applicationReferences.Count + _additionalMetadataReferences.Count);
            compilationReferences.AddRange(applicationReferences);
            compilationReferences.AddRange(_additionalMetadataReferences);

            return compilationReferences;
        }
    }
}
