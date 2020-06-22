﻿using System;

namespace UnityEngine.Perception.GroundTruth {
    [AddComponentMenu("Perception/Labelers/SemanticSegmentationLabeler")]
    [RequireComponent(typeof(InstanceSegmentationLabeler))]
    public class SemanticSegmentationLabeler : MonoBehaviour
    {
        public string annotationId = "12F94D8D-5425-4DEB-9B21-5E53AD957D66";
        public LabelingConfiguration labelingConfiguration;
    }
}