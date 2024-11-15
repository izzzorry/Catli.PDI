from flask import Flask, request, jsonify, url_for
import tensorflow as tf
import cv2
import numpy as np
import os

# Inicializar la app Flask
app = Flask(__name__)
app.static_folder = "segmented_results"

# Cargar el modelo de clasificación
model_path = "modelo_colibri_gato_mariposa.h5"
modelo = tf.keras.models.load_model(model_path)

# Mapeo de clases
class_labels = {0: "Colibrí", 1: "Gato", 2: "Mariposa"}

# Carpeta para guardar imágenes segmentadas
SEGMENTED_FOLDER = "segmented_results"
os.makedirs(SEGMENTED_FOLDER, exist_ok=True)

@app.errorhandler(Exception)
def handle_exception(e):
    """Maneja errores globales y devuelve una respuesta JSON."""
    return jsonify({"error": f"Error interno del servidor: {str(e)}"}), 500

def classify_image(image):
    """Clasifica una imagen utilizando el modelo cargado."""
    try:
        resized_image = cv2.resize(image, (150, 150)) / 255.0
        resized_image = np.expand_dims(resized_image, axis=0)
        predictions = modelo.predict(resized_image)
        predicted_class = np.argmax(predictions)
        return class_labels.get(predicted_class, "Clase desconocida")
    except Exception as e:
        raise ValueError(f"Error durante la clasificación: {str(e)}")

def segment_image(image):
    """Segmenta el objeto principal de la imagen."""
    try:
        gray = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)
        blurred = cv2.GaussianBlur(gray, (5, 5), 0)
        binary_mask = cv2.adaptiveThreshold(
            blurred, 255, cv2.ADAPTIVE_THRESH_GAUSSIAN_C,
            cv2.THRESH_BINARY_INV, blockSize=11, C=2
        )
        contours, _ = cv2.findContours(binary_mask, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)
        if not contours:
            raise ValueError("No se encontraron contornos en la imagen.")
        largest_contour = max(contours, key=cv2.contourArea)
        mask = np.zeros_like(binary_mask)
        cv2.drawContours(mask, [largest_contour], -1, 255, -1)
        segmented = cv2.bitwise_and(image, image, mask=mask)
        b, g, r = cv2.split(segmented)
        alpha = mask
        result = cv2.merge([b, g, r, alpha])
        output_path = os.path.join(SEGMENTED_FOLDER, "segmented_image.png")
        cv2.imwrite(output_path, result)
        return output_path
    except Exception as e:
        raise ValueError(f"Error durante la segmentación: {str(e)}")

@app.route('/procesar', methods=['POST'])
def process_image():
    """Maneja la subida de imágenes, clasifica y segmenta."""
    try:
        if 'file' not in request.files:
            return jsonify({"error": "No se subió ningún archivo"}), 400
        file = request.files['file']
        image = np.frombuffer(file.read(), np.uint8)
        image = cv2.imdecode(image, cv2.IMREAD_COLOR)
        predicted_class = classify_image(image)
        segmented_image_path = segment_image(image)
        segmented_image_url = url_for('static', filename=os.path.basename(segmented_image_path), _external=True)
        return jsonify({
            "class": predicted_class,
            "segmented_image_url": segmented_image_url
        })
    except Exception as e:
        return jsonify({"error": f"Error procesando la imagen: {str(e)}"}), 500

if __name__ == "__main__":
    app.run(host="0.0.0.0", port=5000, debug=True)
