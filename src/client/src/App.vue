<template>
  <div class="app-container">
    <div v-if="loading" class="loading-container">
      <div class="loading-content">
        <div class="loading-spinner">
          <div class="spinner-circle"></div>
          <div class="spinner-circle"></div>
          <div class="spinner-circle"></div>
        </div>
        <h2 class="loading-text">İşbirliği Ağı Yükleniyor</h2>
        <p class="loading-subtext">Veriler hazırlanıyor, lütfen bekleyin...</p>
        <div class="loading-progress">
          <div class="progress-bar"></div>
        </div>
      </div>
    </div>

    <div v-else-if="error" class="error-container">
      <div class="error-content">
        <div class="error-icon">⚠️</div>
        <h2 class="error-title">Bir Hata Oluştu</h2>
        <p class="error-message">{{ error }}</p>
        <button class="retry-button" @click="fetchData">
          <span class="retry-icon">↺</span>
          Tekrar Dene
        </button>
      </div>
    </div>

    <collaboration-graph
      v-else-if="graphData"
      :graph-data="graphData"
      class="graph-component"
    />
  </div>
</template>

<script>
import { ref, onMounted } from 'vue';
import CollaborationGraph from './components/CollaborationGraph.vue';

export default {
  name: 'App',
  components: {
    CollaborationGraph
  },
  setup() {
    const graphData = ref(null);
    const loading = ref(true);
    const error = ref(null);

    const fetchData = async () => {
      try {
        loading.value = true;
        error.value = null;

        const response = await fetch('https://localhost:7088/Main/graph-data');
        if (!response.ok) {
          throw new Error(`Sunucu hatası! Durum kodu: ${response.status}`);
        }
        const data = await response.json();

        if (!data || !data.authors) {
          throw new Error('Geçersiz veri yapısı alındı');
        }

        graphData.value = data;
      } catch (err) {
        console.error('Veri yükleme hatası:', err);
        error.value = `Veri yüklenirken bir hata oluştu: ${err.message}`;
      } finally {
        loading.value = false;
      }
    };

    onMounted(() => {
      fetchData();
    });

    return {
      graphData,
      loading,
      error,
      fetchData
    };
  }
};
</script>

<style scoped>
.app-container {
  min-height: 100vh;
  background: linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%);
}

.loading-container {
  min-height: 100vh;
  display: flex;
  justify-content: center;
  align-items: center;
  background: linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%);
}

.loading-content {
  background: white;
  padding: 40px;
  border-radius: 16px;
  box-shadow: 0 8px 30px rgba(0, 0, 0, 0.12);
  text-align: center;
  max-width: 400px;
  width: 90%;
}

.loading-spinner {
  display: flex;
  justify-content: center;
  gap: 8px;
  margin-bottom: 24px;
}

.spinner-circle {
  width: 12px;
  height: 12px;
  background-color: #1a237e;
  border-radius: 50%;
  animation: bounce 0.5s alternate infinite;
}

.spinner-circle:nth-child(2) {
  animation-delay: 0.16s;
}

.spinner-circle:nth-child(3) {
  animation-delay: 0.32s;
}

@keyframes bounce {
  from {
    transform: translateY(0);
  }
  to {
    transform: translateY(-12px);
  }
}

.loading-text {
  color: #1a237e;
  font-size: 24px;
  margin: 0 0 12px;
  font-weight: 600;
}

.loading-subtext {
  color: #666;
  margin: 0 0 24px;
  font-size: 16px;
}

.loading-progress {
  background: #f0f0f0;
  height: 4px;
  border-radius: 2px;
  overflow: hidden;
}

.progress-bar {
  height: 100%;
  background: #1a237e;
  animation: progress 2s infinite;
  transform-origin: left;
}

@keyframes progress {
  0% {
    transform: scaleX(0);
  }
  50% {
    transform: scaleX(0.7);
  }
  100% {
    transform: scaleX(0.9);
  }
}

.error-container {
  min-height: 100vh;
  display: flex;
  justify-content: center;
  align-items: center;
  background: linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%);
}

.error-content {
  background: white;
  padding: 40px;
  border-radius: 16px;
  box-shadow: 0 8px 30px rgba(0, 0, 0, 0.12);
  text-align: center;
  max-width: 400px;
  width: 90%;
}

.error-icon {
  font-size: 48px;
  margin-bottom: 16px;
}

.error-title {
  color: #d32f2f;
  font-size: 24px;
  margin: 0 0 12px;
  font-weight: 600;
}

.error-message {
  color: #666;
  margin: 0 0 24px;
  font-size: 16px;
  line-height: 1.5;
}

.retry-button {
  background: #1a237e;
  color: white;
  border: none;
  padding: 12px 24px;
  border-radius: 8px;
  font-size: 16px;
  font-weight: 500;
  cursor: pointer;
  display: inline-flex;
  align-items: center;
  gap: 8px;
  transition: all 0.3s ease;
}

.retry-button:hover {
  background: #283593;
  transform: translateY(-2px);
}

.retry-icon {
  font-size: 20px;
}

.graph-component {
  opacity: 0;
  animation: fadeIn 0.5s ease-out forwards;
}

@keyframes fadeIn {
  from {
    opacity: 0;
  }
  to {
    opacity: 1;
  }
}

@media (max-width: 480px) {
  .loading-content,
  .error-content {
    padding: 30px;
  }

  .loading-text,
  .error-title {
    font-size: 20px;
  }

  .loading-subtext,
  .error-message {
    font-size: 14px;
  }
}
</style>
