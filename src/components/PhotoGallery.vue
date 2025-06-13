<template>
  <div class="photo-gallery">
    <div class="gallery-header">
      <h2 class="gallery-title">
        <span class="title-icon">📸</span>
        Galeria de Momentos Especiais
      </h2>
      <p class="gallery-description">
        Adicione e preserve suas memórias mais preciosas
      </p>
    </div>

    <div class="upload-section">
      <div class="upload-area" @click="triggerFileInput" @dragover.prevent @drop.prevent="handleDrop">
        <input 
          ref="fileInput" 
          type="file" 
          multiple 
          accept="image/*" 
          @change="handleFileSelect"
          style="display: none"
        >
        <div class="upload-content">
          <div class="upload-icon-container">
            <div class="upload-icon">📷</div>
            <div class="click-indicator">👆</div>
          </div>
          <h3 class="upload-title">Adicionar Fotos</h3>
          <div class="upload-instructions">
            <p class="primary-instruction">
              <strong>👆 CLIQUE AQUI</strong> para selecionar suas fotos
            </p>
            <p class="secondary-instruction">
              ou arraste e solte suas imagens nesta área
            </p>
          </div>
          <div class="upload-details">
            <span class="detail-item">📁 Formatos: JPG, PNG, GIF</span>
            <span class="detail-item">📏 Máximo: 5MB cada</span>
            <span class="detail-item">🖼️ Múltiplas fotos aceitas</span>
          </div>
          <div class="upload-button-visual">
            <span class="button-text">Clique para Escolher Fotos</span>
          </div>
        </div>
      </div>
    </div>

    <div v-if="photos.length > 0" class="photos-grid">
      <div 
        v-for="(photo, index) in photos" 
        :key="photo.id"
        class="photo-item"
        @click="openLightbox(index)"
      >
        <img :src="photo.url" :alt="photo.name" class="photo-image">
        <div class="photo-overlay">
          <div class="photo-actions">
            <button @click.stop="downloadPhoto(photo)" class="action-btn download-btn" title="Baixar">
              📥
            </button>
            <button @click.stop="deletePhoto(photo.id)" class="action-btn delete-btn" title="Excluir">
              🗑️
            </button>
          </div>
          <div class="photo-info">
            <span class="photo-name">{{ photo.name }}</span>
            <span class="photo-date">{{ formatDate(photo.uploadDate) }}</span>
          </div>
        </div>
      </div>
    </div>

    <div v-else class="empty-gallery">
      <div class="empty-icon">🖼️</div>
      <h3>Nenhuma foto ainda</h3>
      <p>Adicione suas primeiras memórias especiais!</p>
      <div class="empty-cta">
        <span class="cta-arrow">👆</span>
        <span class="cta-text">Clique na área acima para começar</span>
      </div>
    </div>

    <!-- Lightbox Modal -->
    <div v-if="lightboxOpen" class="lightbox-overlay" @click="closeLightbox">
      <div class="lightbox-content" @click.stop>
        <button class="lightbox-close" @click="closeLightbox">×</button>
        <button class="lightbox-nav prev" @click="previousPhoto" v-if="photos.length > 1">‹</button>
        <button class="lightbox-nav next" @click="nextPhoto" v-if="photos.length > 1">›</button>
        
        <img 
          :src="photos[currentPhotoIndex]?.url" 
          :alt="photos[currentPhotoIndex]?.name"
          class="lightbox-image"
        >
        
        <div class="lightbox-info">
          <h3>{{ photos[currentPhotoIndex]?.name }}</h3>
          <p>{{ formatDate(photos[currentPhotoIndex]?.uploadDate) }}</p>
          <span class="photo-counter">{{ currentPhotoIndex + 1 }} de {{ photos.length }}</span>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'

interface Photo {
  id: string
  name: string
  url: string
  uploadDate: Date
  file?: File
}

const photos = ref<Photo[]>([])
const fileInput = ref<HTMLInputElement>()
const lightboxOpen = ref(false)
const currentPhotoIndex = ref(0)

// Carrega fotos salvas do localStorage
onMounted(() => {
  loadPhotosFromStorage()
})

const loadPhotosFromStorage = () => {
  const savedPhotos = localStorage.getItem('gallery-photos')
  if (savedPhotos) {
    photos.value = JSON.parse(savedPhotos)
  }
}

const savePhotosToStorage = () => {
  localStorage.setItem('gallery-photos', JSON.stringify(photos.value))
}

const triggerFileInput = () => {
  fileInput.value?.click()
}

const handleFileSelect = (event: Event) => {
  const target = event.target as HTMLInputElement
  if (target.files) {
    handleFiles(Array.from(target.files))
  }
}

const handleDrop = (event: DragEvent) => {
  if (event.dataTransfer?.files) {
    handleFiles(Array.from(event.dataTransfer.files))
  }
}

const handleFiles = (files: File[]) => {
  files.forEach(file => {
    if (file.type.startsWith('image/') && file.size <= 5 * 1024 * 1024) {
      const reader = new FileReader()
      reader.onload = (e) => {
        const photo: Photo = {
          id: generateId(),
          name: file.name,
          url: e.target?.result as string,
          uploadDate: new Date(),
          file
        }
        photos.value.push(photo)
        savePhotosToStorage()
      }
      reader.readAsDataURL(file)
    } else {
      alert(`Arquivo ${file.name} é muito grande ou não é uma imagem válida.`)
    }
  })
}

const generateId = (): string => {
  return Date.now().toString(36) + Math.random().toString(36).substr(2)
}

const deletePhoto = (photoId: string) => {
  if (confirm('Tem certeza que deseja excluir esta foto?')) {
    photos.value = photos.value.filter(photo => photo.id !== photoId)
    savePhotosToStorage()
  }
}

const downloadPhoto = (photo: Photo) => {
  const link = document.createElement('a')
  link.href = photo.url
  link.download = photo.name
  link.click()
}

const openLightbox = (index: number) => {
  currentPhotoIndex.value = index
  lightboxOpen.value = true
  document.body.style.overflow = 'hidden'
}

const closeLightbox = () => {
  lightboxOpen.value = false
  document.body.style.overflow = 'auto'
}

const nextPhoto = () => {
  currentPhotoIndex.value = (currentPhotoIndex.value + 1) % photos.value.length
}

const previousPhoto = () => {
  currentPhotoIndex.value = currentPhotoIndex.value === 0 
    ? photos.value.length - 1 
    : currentPhotoIndex.value - 1
}

const formatDate = (date: Date | string): string => {
  const d = typeof date === 'string' ? new Date(date) : date
  return d.toLocaleDateString('pt-BR', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}
</script>

<style scoped>
.photo-gallery {
  width: 100%;
  max-width: 1200px;
  margin: 0 auto;
  padding: 2rem;
}

.gallery-header {
  text-align: center;
  margin-bottom: 3rem;
}

.gallery-title {
  font-size: 2.5rem;
  font-weight: 700;
  color: var(--text-primary);
  margin-bottom: 1rem;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 1rem;
}

.title-icon {
  font-size: 2rem;
}

.gallery-description {
  font-size: 1.1rem;
  color: var(--text-secondary);
  line-height: 1.6;
}

.upload-section {
  margin-bottom: 3rem;
}

.upload-area {
  border: 3px dashed var(--primary-color);
  border-radius: 20px;
  padding: 3rem;
  text-align: center;
  cursor: pointer;
  transition: all 0.3s ease;
  background: var(--upload-bg);
  position: relative;
  overflow: hidden;
}

.upload-area::before {
  content: '';
  position: absolute;
  top: 0;
  left: -100%;
  width: 100%;
  height: 100%;
  background: linear-gradient(90deg, transparent, rgba(236, 72, 153, 0.1), transparent);
  animation: shimmer 2s infinite;
}

.upload-area:hover {
  border-color: var(--primary-color);
  background: var(--upload-hover-bg);
  transform: translateY(-2px);
  box-shadow: 0 10px 30px var(--shadow-color);
}

.upload-content {
  position: relative;
  z-index: 2;
}

.upload-icon-container {
  position: relative;
  display: inline-block;
  margin-bottom: 1.5rem;
}

.upload-icon {
  font-size: 4rem;
  margin-bottom: 0.5rem;
  animation: bounce 2s infinite;
}

.click-indicator {
  position: absolute;
  top: -10px;
  right: -10px;
  font-size: 1.5rem;
  animation: pulse-point 1.5s infinite;
}

.upload-title {
  font-size: 1.8rem;
  color: var(--text-primary);
  margin-bottom: 1rem;
  font-weight: 700;
}

.upload-instructions {
  margin-bottom: 1.5rem;
}

.primary-instruction {
  font-size: 1.2rem;
  color: var(--primary-color);
  margin-bottom: 0.5rem;
  font-weight: 700;
  animation: glow 2s ease-in-out infinite alternate;
}

.secondary-instruction {
  color: var(--text-secondary);
  font-size: 1rem;
}

.upload-details {
  display: flex;
  justify-content: center;
  gap: 1rem;
  flex-wrap: wrap;
  margin-bottom: 1.5rem;
}

.detail-item {
  background: var(--detail-bg);
  padding: 0.5rem 1rem;
  border-radius: 20px;
  font-size: 0.9rem;
  color: var(--text-secondary);
  border: 1px solid var(--detail-border);
}

.upload-button-visual {
  background: linear-gradient(135deg, var(--primary-color), #8B5CF6);
  color: white;
  padding: 1rem 2rem;
  border-radius: 25px;
  display: inline-block;
  font-weight: 600;
  font-size: 1.1rem;
  box-shadow: 0 5px 15px rgba(236, 72, 153, 0.3);
  transition: all 0.3s ease;
  animation: button-pulse 2s infinite;
}

.upload-area:hover .upload-button-visual {
  transform: scale(1.05);
  box-shadow: 0 8px 25px rgba(236, 72, 153, 0.4);
}

.photos-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
  gap: 2rem;
}

.photo-item {
  position: relative;
  aspect-ratio: 1;
  border-radius: 15px;
  overflow: hidden;
  cursor: pointer;
  transition: all 0.3s ease;
  box-shadow: 0 8px 25px var(--shadow-color);
}

.photo-item:hover {
  transform: translateY(-5px);
  box-shadow: 0 15px 35px var(--shadow-hover-color);
}

.photo-image {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.3s ease;
}

.photo-item:hover .photo-image {
  transform: scale(1.05);
}

.photo-overlay {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: linear-gradient(
    to bottom,
    rgba(0, 0, 0, 0.1) 0%,
    rgba(0, 0, 0, 0.1) 60%,
    rgba(0, 0, 0, 0.8) 100%
  );
  opacity: 0;
  transition: opacity 0.3s ease;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  padding: 1rem;
}

.photo-item:hover .photo-overlay {
  opacity: 1;
}

.photo-actions {
  display: flex;
  gap: 0.5rem;
  justify-content: flex-end;
}

.action-btn {
  background: rgba(255, 255, 255, 0.9);
  border: none;
  border-radius: 50%;
  width: 40px;
  height: 40px;
  cursor: pointer;
  font-size: 1.2rem;
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  justify-content: center;
}

.action-btn:hover {
  background: white;
  transform: scale(1.1);
}

.delete-btn:hover {
  background: #ff4757;
  color: white;
}

.download-btn:hover {
  background: #2ed573;
  color: white;
}

.photo-info {
  color: white;
  text-align: left;
}

.photo-name {
  display: block;
  font-weight: 600;
  margin-bottom: 0.25rem;
  font-size: 0.9rem;
}

.photo-date {
  font-size: 0.8rem;
  opacity: 0.8;
}

.empty-gallery {
  text-align: center;
  padding: 4rem 2rem;
  color: var(--text-secondary);
}

.empty-icon {
  font-size: 4rem;
  margin-bottom: 1rem;
  opacity: 0.5;
}

.empty-gallery h3 {
  font-size: 1.5rem;
  margin-bottom: 0.5rem;
  color: var(--text-primary);
}

.empty-cta {
  margin-top: 2rem;
  padding: 1rem;
  background: var(--cta-bg);
  border-radius: 15px;
  border: 2px dashed var(--primary-color);
}

.cta-arrow {
  font-size: 1.5rem;
  display: block;
  margin-bottom: 0.5rem;
  animation: bounce-arrow 1.5s infinite;
}

.cta-text {
  font-weight: 600;
  color: var(--primary-color);
}

/* Lightbox */
.lightbox-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.9);
  z-index: 2000;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 2rem;
}

.lightbox-content {
  position: relative;
  max-width: 90vw;
  max-height: 90vh;
  display: flex;
  flex-direction: column;
  align-items: center;
}

.lightbox-image {
  max-width: 100%;
  max-height: 70vh;
  object-fit: contain;
  border-radius: 10px;
  box-shadow: 0 20px 40px rgba(0, 0, 0, 0.5);
}

.lightbox-close {
  position: absolute;
  top: -50px;
  right: -10px;
  background: rgba(255, 255, 255, 0.2);
  border: none;
  color: white;
  font-size: 2rem;
  width: 40px;
  height: 40px;
  border-radius: 50%;
  cursor: pointer;
  transition: all 0.3s ease;
}

.lightbox-close:hover {
  background: rgba(255, 255, 255, 0.3);
}

.lightbox-nav {
  position: absolute;
  top: 50%;
  transform: translateY(-50%);
  background: rgba(255, 255, 255, 0.2);
  border: none;
  color: white;
  font-size: 2rem;
  width: 50px;
  height: 50px;
  border-radius: 50%;
  cursor: pointer;
  transition: all 0.3s ease;
}

.lightbox-nav:hover {
  background: rgba(255, 255, 255, 0.3);
}

.lightbox-nav.prev {
  left: -70px;
}

.lightbox-nav.next {
  right: -70px;
}

.lightbox-info {
  text-align: center;
  color: white;
  margin-top: 1rem;
}

.lightbox-info h3 {
  margin-bottom: 0.5rem;
}

.photo-counter {
  display: block;
  margin-top: 0.5rem;
  opacity: 0.7;
  font-size: 0.9rem;
}

/* Animações */
@keyframes shimmer {
  0% { left: -100%; }
  100% { left: 100%; }
}

@keyframes bounce {
  0%, 20%, 50%, 80%, 100% {
    transform: translateY(0);
  }
  40% {
    transform: translateY(-10px);
  }
  60% {
    transform: translateY(-5px);
  }
}

@keyframes pulse-point {
  0%, 100% {
    transform: scale(1);
    opacity: 1;
  }
  50% {
    transform: scale(1.2);
    opacity: 0.7;
  }
}

@keyframes glow {
  from {
    text-shadow: 0 0 5px var(--primary-color);
  }
  to {
    text-shadow: 0 0 20px var(--primary-color), 0 0 30px var(--primary-color);
  }
}

@keyframes button-pulse {
  0%, 100% {
    transform: scale(1);
  }
  50% {
    transform: scale(1.02);
  }
}

@keyframes bounce-arrow {
  0%, 20%, 50%, 80%, 100% {
    transform: translateY(0);
  }
  40% {
    transform: translateY(-8px);
  }
  60% {
    transform: translateY(-4px);
  }
}

/* Responsive */
@media (max-width: 768px) {
  .photo-gallery {
    padding: 1rem;
  }
  
  .gallery-title {
    font-size: 2rem;
  }
  
  .photos-grid {
    grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
    gap: 1rem;
  }
  
  .upload-area {
    padding: 2rem 1rem;
  }
  
  .upload-details {
    flex-direction: column;
    align-items: center;
  }
  
  .lightbox-nav {
    display: none;
  }
  
  .lightbox-close {
    top: -40px;
    right: 0;
  }
}

/* Variáveis CSS para temas */
:root {
  --text-primary: #374151;
  --text-secondary: #6B7280;
  --text-tertiary: #9CA3AF;
  --primary-color: #EC4899;
  --border-color: #E5E7EB;
  --upload-bg: rgba(255, 255, 255, 0.1);
  --upload-hover-bg: rgba(255, 255, 255, 0.2);
  --shadow-color: rgba(0, 0, 0, 0.1);
  --shadow-hover-color: rgba(0, 0, 0, 0.15);
  --detail-bg: rgba(255, 255, 255, 0.5);
  --detail-border: rgba(255, 255, 255, 0.3);
  --cta-bg: rgba(255, 255, 255, 0.1);
}

:global(.dark-theme) {
  --text-primary: #F9FAFB;
  --text-secondary: #D1D5DB;
  --text-tertiary: #9CA3AF;
  --primary-color: #EC4899;
  --border-color: #374151;
  --upload-bg: rgba(0, 0, 0, 0.2);
  --upload-hover-bg: rgba(0, 0, 0, 0.3);
  --shadow-color: rgba(0, 0, 0, 0.3);
  --shadow-hover-color: rgba(0, 0, 0, 0.4);
  --detail-bg: rgba(0, 0, 0, 0.3);
  --detail-border: rgba(255, 255, 255, 0.1);
  --cta-bg: rgba(0, 0, 0, 0.2);
}
</style>