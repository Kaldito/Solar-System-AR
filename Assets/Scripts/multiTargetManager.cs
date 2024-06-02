using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class multiTargetManager : MonoBehaviour
{
    // ------------------ CONSTANTS SETUP ------------------ // 
    // Creamos una variable para el ARTrackedImageManager
    [SerializeField] private ARTrackedImageManager _arTrackedImageManager; 
    // Creamos un diccionario para guardar los modelos 3D
    [SerializeField] private GameObject[] _arModels;

    // Creamos un diccionario para guardar los modelos 3D con su nombre
    private Dictionary<string, GameObject> _arModelsDict = new Dictionary<string, GameObject>();
    // Creamos un diccionario para guardar los modelos 3D con su nombre y si está activo
    private Dictionary<string, bool> modelState = new Dictionary<string, bool>();

    // ------------------ FUNCION DE INICIO ------------------ //
    // - Esta función se ejecuta al inicio del programa - //
    // - En esta función se inicializan los modelos 3D - //
    void Start()
    {
        foreach (var _aRmodel in _arModels)
        {
            GameObject newARModel = Instantiate(_aRmodel, Vector3.zero, Quaternion.identity);
            // Inicializamos los modelos 3D
            // Vector3.zero nos indica que el modelo 3D se inicializa en la posición 0,0,0
            // Quaternion.identity nos indica que el modelo 3D se inicializa con la rotación 0,0,0
            newARModel.name = _aRmodel.name; // Asignamos el nombre del modelo 3D
            _arModelsDict.Add(_aRmodel.name, newARModel); // Añadimos el modelo 3D al diccionario
            newARModel.SetActive(false); // Desactivamos el modelo 3D
            modelState.Add(_aRmodel.name, false); // Añadimos el modelo 3D al diccionario de estado
        }
    }

    // ------------------ FUNCION DE MOSTRAR MODELOS 3D ------------------ //
    private void ShowARModel(ARTrackedImage trackedImage)
    {   
        string trackedImageName = trackedImage.referenceImage.name; // Obtenemos el nombre de la imagen detectada
        bool isModelActive = modelState[trackedImageName]; // Obtenemos el estado del modelo 3D
        if (!isModelActive) // Si el modelo 3D no está activo
        {
            // - Mostramos el modelo 3D si no se habia mostrado antes - //
            GameObject _arModel = _arModelsDict[trackedImageName]; // Obtenemos el modelo 3D

            // Asignamos la posición del modelo 3D a la posición de la imagen detectada y le agregamos un offset en Y
            _arModel.transform.position = trackedImage.transform.position; // Asignamos la posición del modelo 3D
            _arModel.SetActive(true); // Activamos el modelo 3D
            modelState[trackedImageName] = true; // Actualizamos el estado del modelo 3D
        }
        else 
        {   
            // - Actualizamos la posición del modelo 3D si ya se había mostrado antes - //
            GameObject _arModel = _arModelsDict[trackedImageName]; // Obtenemos el modelo 3D
            _arModel.transform.position = trackedImage.transform.position; // Asignamos la posición del modelo 3D
        }
    }

    // ------------------ FUNCION DE OCULTAR MODELOS 3D ------------------ //
    private void HideARModel(ARTrackedImage trackedImage)
    {
        string trackedImageName = trackedImage.referenceImage.name; // Obtenemos el nombre de la imagen detectada
        bool isModelActive = modelState[trackedImageName]; // Obtenemos el estado del modelo 3D
        if (isModelActive) // Si el modelo 3D no está activo
        {
            GameObject _arModel = _arModelsDict[trackedImageName]; // Obtenemos el modelo 3D
            _arModel.SetActive(false); // Activamos el modelo 3D
            modelState[trackedImageName] = false; // Actualizamos el estado del modelo 3D
        }
    }

    // ------------------ FUNCION DE IMAGEN DETECTADA ------------------ //
    private void ImageFound(ARTrackedImagesChangedEventArgs eventData)
    {
        foreach (var trackedImage in eventData.added) // En eventData.added se encuentran las imagenes detectadas
        {
            ShowARModel(trackedImage); // Mostramos el modelo 3D
        }

        foreach (var trackedImage in eventData.updated) // En eventData.updated se encuentran las imagenes actualizadas
        {
            if (trackedImage.trackingState == TrackingState.Tracking) // TrackingState.Tracking nos indica que la imagen se está siguiendo
            {
                ShowARModel(trackedImage); // Mostramos el modelo 3D
            } 
        }
    }

    // ------------------ FUNCION DE ACTIVACION DE EVENTOS ------------------ //
    // - Esta función se ejecuta al activar el script - //
    private void OnEnable()
    {
        // Suscribimos el evento de la imagen detectada
        _arTrackedImageManager.trackedImagesChanged += ImageFound;
    }

    // ------------------ FUNCION DE DESACTIVACION DE EVENTOS ------------------ //
    // - Esta función se ejecuta al desactivar el script - //
    private void OnDisable()
    {
        // Desuscribimos el evento de la imagen detectada
        _arTrackedImageManager.trackedImagesChanged -= ImageFound;
    }
}
