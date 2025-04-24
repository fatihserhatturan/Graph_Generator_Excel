<template>
  <div class="graph-wrapper">
    <div class="graph-header">
      <h2>Yazar İşbirliği Ağı</h2>
      <div class="header-controls">
        <div class="graph-stats">
          <div class="stat-item">
            <span class="stat-label">Toplam Yazar:</span>
            <span class="stat-value">{{ graphData.authors.length }}</span>
          </div>
          <div class="stat-item">
            <span class="stat-label">Ortalama Makale:</span>
            <span class="stat-value">{{ averagePapers.toFixed(1) }}</span>
          </div>
        </div>
        <div class="dropdown-container" ref="dropdownRef">
          <button class="dropdown-btn" @click="toggleDropdown">
            İsterler
            <span class="dropdown-arrow" :class="{ 'arrow-up': isDropdownOpen }">▼</span>
          </button>
          <div v-show="isDropdownOpen" class="dropdown-content">
            <div class="dropdown-section">
              <h3>En Uzun Yol Bulma</h3>
              <div class="input-group">
                <input
                  v-model="searchAuthorId"
                  type="number"
                  class="dropdown-input"
                  placeholder="Yazar ID"
                  @keyup.enter="findLongestPath"
                />
                <button class="dropdown-action-btn" @click="findLongestPath">
                  <font-awesome-icon :icon="['fas', 'share-nodes']" class="btn-icon" />
                </button>
              </div>
            </div>
            <div class="dropdown-section">
              <h3>İşbirliği Sayısı Hesaplama</h3>
              <div class="input-group">
                <input
                  v-model="collaborationSearchId"
                  type="number"
                  class="dropdown-input"
                  placeholder="Yazar ID"
                  @keyup.enter="calculateCollaborations"
                />
                <button class="dropdown-action-btn" @click="calculateCollaborations">
                  <font-awesome-icon :icon="['fas', 'handshake-simple']" class="btn-icon" />
                </button>
              </div>
            </div>
            <div class="dropdown-section">
              <h3>En Çok İşbirliği Yapan Yazar</h3>

              <button class="dropdown-action-btn full-width" @click="showMostCollaborative">
                <font-awesome-icon :icon="['fas', 'user-group']" class="btn-icon" />
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
    <div id="cy" ref="cyElement" class="graph-container"></div>
    <div class="controls-panel">
      <button class="control-btn" @click="zoomIn">
        <font-awesome-icon :icon="['fas', 'magnifying-glass-plus']" class="btn-icon" />
        <span class="btn-text">Yakınlaştır</span>
      </button>
      <button class="control-btn" @click="zoomOut">
        <font-awesome-icon :icon="['fas', 'magnifying-glass-minus']" class="btn-icon" />
        <span class="btn-text">Uzaklaştır</span>
      </button>
      <button class="control-btn" @click="fit">
        <font-awesome-icon :icon="['fas', 'maximize']" class="btn-icon" />
        <span class="btn-text">Sığdır</span>
      </button>

    </div>
    <div v-if="selectedNode" class="node-details">
      <div class="details-header">
        <h3>{{ selectedNode.label }}</h3>
        <button class="close-btn" @click="closeDetails">×</button>
      </div>
      <div class="details-content">
        <p class="detail-item">
          <strong>Yazar ID:</strong> {{ selectedNode.id }}
        </p>
        <p class="detail-item">
          <strong>Makale Sayısı:</strong> {{ selectedNode.papers.length }}
        </p>
        <p class="detail-item">
          <strong>İşbirliği Sayısı:</strong> {{ selectedNode.collaborationCount }}
        </p>
        <div v-if="selectedNode.pathInfo" class="path-info">
          <h4>En Uzun Yol Bilgisi</h4>
          <p>Yol Uzunluğu: {{ selectedNode.pathInfo.length }} düğüm</p>
          <div class="path-nodes">
            <h5>Yol Üzerindeki Yazarlar:</h5>
            <ul>
              <li v-for="(node, index) in selectedNode.pathInfo.nodes" :key="index">
                {{ index + 1 }}. {{ node }}
              </li>
            </ul>
          </div>
        </div>
        <div v-else-if="selectedNode.isCollaborationView" class="collaboration-info">
          <h4>İşbirliği Bilgileri</h4>
          <p class="collaboration-count">Toplam İşbirliği: {{ selectedNode.collaborationCount }} yazar</p>
          <div class="collaborators-list">
            <h5>İşbirliği Yapılan Yazarlar:</h5>
            <ul>
              <li v-for="collaborator in selectedNode.collaborators" :key="collaborator.id">
                {{ collaborator.name }} (ID: {{ collaborator.id }})
              </li>
            </ul>
          </div>
        </div>
        <div v-else class="papers-list">
          <h4>Makaleler</h4>
          <ul>
            <li v-for="(paper, index) in selectedNode.papers.slice(0, 5)" :key="index">
              {{ paper }}
            </li>
          </ul>
          <p v-if="selectedNode.papers.length > 5" class="more-papers">
            ...ve {{ selectedNode.papers.length - 5 }} makale daha
          </p>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import cytoscape from 'cytoscape';
import { onMounted, ref, computed } from 'vue';

export default {
  name: 'CollaborationGraph',
  props: {
    graphData: {
      type: Object,
      required: true
    }
  },
  setup(props) {
    const cyElement = ref(null);
    const selectedNode = ref(null);
    const searchAuthorId = ref('');
    const collaborationSearchId = ref('');
    const isDropdownOpen = ref(false);
    const dropdownRef = ref(null);
    let cy = null;

    // Dropdown dışına tıklandığında kapanması için
    onMounted(() => {
      document.addEventListener('click', handleClickOutside);
      initializeCytoscape();
    });

    const handleClickOutside = (event) => {
      if (dropdownRef.value && !dropdownRef.value.contains(event.target)) {
        isDropdownOpen.value = false;
      }
    };

    const toggleDropdown = () => {
      isDropdownOpen.value = !isDropdownOpen.value;
    };

    const averagePapers = computed(() => {
      const totalPapers = props.graphData.authors.reduce((sum, author) => sum + author.totalPapers, 0);
      return totalPapers / props.graphData.authors.length;
    });

    const generateGradientColor = (performance) => {
      if (performance === 'high') return '#00c853';
      if (performance === 'low') return '#ff5252';
      return '#2196f3';
    };

    const initializeCytoscape = () => {
      const paperCounts = props.graphData.authors.map(a => a.totalPapers);
      const maxPapers = Math.max(...paperCounts);
      const minPapers = Math.min(...paperCounts);

      const normalizeSize = (papers) => {
        return 30 + ((papers - minPapers) / (maxPapers - minPapers)) * 50;
      };

      const nodes = props.graphData.authors.map(author => ({
        data: {
          id: author.id.toString(),
          label: author.name,
          papers: author.papers,
          size: normalizeSize(author.totalPapers),
          performance: author.isHighPerforming ? 'high' : (author.isLowPerforming ? 'low' : 'normal'),
          collaborationCount: Object.keys(author.collaborations).length
        }
      }));

      const edges = [];
      const weights = [];
      props.graphData.authors.forEach(author => {
        Object.entries(author.collaborations).forEach(([collaboratorId, weight]) => {
          if (parseInt(author.id) < parseInt(collaboratorId)) {
            weights.push(weight);
            edges.push({
              data: {
                id: `${author.id}-${collaboratorId}`,
                source: author.id.toString(),
                target: collaboratorId.toString(),
                weight: weight
              }
            });
          }
        });
      });

      const maxWeight = Math.max(...weights);
      const minWeight = Math.min(...weights);

      cy = cytoscape({
        container: cyElement.value,
        elements: { nodes, edges },
        style: [
          {
            selector: 'node',
            style: {
              'background-color': ele => generateGradientColor(ele.data('performance')),
              'width': 'data(size)',
              'height': 'data(size)',
              'label': 'data(label)',
              'font-size': '12px',
              'text-valign': 'center',
              'text-halign': 'center',
              'text-outline-width': 3,
              'text-outline-color': '#fff',
              'text-outline-opacity': 0.9,
              'color': '#000',
              'min-zoomed-font-size': '8px',
              'border-width': '3px',
              'border-color': '#fff',
              'border-opacity': 0.8,
              'shadow-blur': '10px',
              'shadow-color': '#000',
              'shadow-opacity': 0.3,
              'shadow-offset-x': '0px',
              'shadow-offset-y': '4px'
            }
          },
          {
            selector: 'edge',
            style: {
              'width': ele => {
                const normalizedWeight = 2 + ((ele.data('weight') - minWeight) / (maxWeight - minWeight)) * 6;
                return normalizedWeight;
              },
              'line-color': '#2196f3',
              'curve-style': 'bezier',
              'opacity': 0.4,
              'line-style': 'solid',
              'target-arrow-shape': 'none',
              'shadow-blur': '5px',
              'shadow-color': '#000',
              'shadow-opacity': 0.2,
              'shadow-offset-x': '0px',
              'shadow-offset-y': '2px'
            }
          },
          {
            selector: 'node:selected',
            style: {
              'border-width': '5px',
              'border-color': '#ff4081',
              'border-opacity': 1,
              'shadow-blur': '15px',
              'shadow-opacity': 0.5
            }
          },
          {
            selector: 'edge:selected',
            style: {
              'line-color': '#ff4081',
              'opacity': 1,
              'width': 'data(weight)',
              'shadow-blur': '10px',
              'shadow-opacity': 0.4
            }
          },
          {
            selector: 'node.highlight',
            style: {
              'border-color': '#ff4081',
              'border-width': '4px',
              'shadow-blur': '15px',
              'shadow-opacity': 0.5
            }
          },
          {
            selector: 'edge.highlight',
            style: {
              'line-color': '#ff4081',
              'opacity': 0.8,
              'shadow-blur': '10px',
              'shadow-opacity': 0.4
            }
          }
        ],
        layout: {
          name: 'cose',
          idealEdgeLength: edge => 120 / edge.data('weight'),
          nodeOverlap: 25,
          refresh: 20,
          fit: true,
          padding: 50,
          randomize: false,
          componentSpacing: 150,
          nodeRepulsion: node => 500000 * (node.data('collaborationCount') || 1),
          edgeElasticity: edge => 100 * edge.data('weight'),
          nestingFactor: 5,
          gravity: 120,
          numIter: 2500,
          initialTemp: 250,
          coolingFactor: 0.95,
          minTemp: 1.0,
          animate: 'end'
        },
        wheelSensitivity: 0.2,
        minZoom: 0.1,
        maxZoom: 3
      });

      cy.on('tap', 'node', function(evt) {
        const node = evt.target;
        selectedNode.value = {
          id: node.data('id'),
          label: node.data('label'),
          papers: node.data('papers'),
          collaborationCount: node.data('collaborationCount')
        };

        cy.elements().removeClass('highlight');
        const neighborhood = node.neighborhood();
        neighborhood.addClass('highlight');
        node.addClass('highlight');
      });

      cy.on('tap', function(evt) {
        if (evt.target === cy) {
          selectedNode.value = null;
          cy.elements().removeClass('highlight');
        }
      });
    };

    const zoomIn = () => {
      if (cy) {
        cy.animate({
          zoom: cy.zoom() * 1.2,
          duration: 300
        });
      }
    };

    const zoomOut = () => {
      if (cy) {
        cy.animate({
          zoom: cy.zoom() * 0.8,
          duration: 300
        });
      }
    };

    const fit = () => {
      if (cy) {
        cy.animate({
          fit: {
            padding: 50
          },
          duration: 500
        });
      }
    };

    const showMostCollaborative = () => {
      if (!cy) return;

      // En çok işbirliği yapan yazarı bul
      const mostCollaborative = props.graphData.authors.reduce((prev, current) => {
        const prevCollabCount = Object.keys(prev.collaborations).length;
        const currentCollabCount = Object.keys(current.collaborations).length;
        return prevCollabCount > currentCollabCount ? prev : current;
      });

      // İlgili node'u bul ve seç
      const node = cy.$id(mostCollaborative.id.toString());
      if (node) {
        // Önceki vurgulamaları temizle
        cy.elements().removeClass('highlight');

        // Yazarı ve bağlantılarını vurgula
        const neighborhood = node.neighborhood();
        neighborhood.addClass('highlight');
        node.addClass('highlight');

        // Detay panelini güncelle
        selectedNode.value = {
          label: mostCollaborative.name,
          papers: mostCollaborative.papers,
          collaborationCount: Object.keys(mostCollaborative.collaborations).length
        };

        // Node'a zoom yap
        cy.animate({
          center: {
            eles: node
          },
          zoom: 1.5,
          duration: 500
        });
      }
    };

    const calculateCollaborations = () => {
      if (!cy || !collaborationSearchId.value) return;

      const authorNode = cy.$id(collaborationSearchId.value);
      if (!authorNode) {
        alert('Yazar bulunamadı!');
        return;
      }

      // Tüm vurgulamaları temizle
      cy.elements().removeClass('highlight');

      // Yazarın bağlantılarını bul
      const neighbors = authorNode.neighborhood('node');
      const collaborationCount = neighbors.length;

      // Yazarı ve bağlantılarını vurgula
      authorNode.addClass('highlight');
      neighbors.addClass('highlight');
      authorNode.neighborhood('edge').addClass('highlight');

      // Detay panelini güncelle
      selectedNode.value = {
        id: authorNode.data('id'),
        label: authorNode.data('label'),
        papers: authorNode.data('papers'),
        collaborationCount: collaborationCount,
        isCollaborationView: true,
        collaborators: neighbors.map(node => ({
          id: node.data('id'),
          name: node.data('label')
        }))
      };

      // Vurgulanan elemanlara zoom yap
      cy.animate({
        fit: {
          eles: cy.elements('.highlight'),
          padding: 50
        },
        duration: 500
      });
    };

    const findLongestPath = () => {
      if (!cy || !searchAuthorId.value) return;

      const startNode = cy.$id(searchAuthorId.value);
      if (!startNode) {
        alert('Yazar bulunamadı!');
        return;
      }

      // Tüm vurgulamaları temizle
      cy.elements().removeClass('highlight');

      // DFS ile en uzun yolu bul
      const visited = new Set();
      let longestPath = [];
      let maxLength = 0;

      const dfs = (node, currentPath) => {
        visited.add(node.id());
        const neighbors = node.neighborhood('node').filter(n => !visited.has(n.id()));

        if (currentPath.length > maxLength) {
          maxLength = currentPath.length;
          longestPath = [...currentPath];
        }

        for (const neighbor of neighbors) {
          currentPath.push(neighbor);
          dfs(neighbor, currentPath);
          currentPath.pop();
        }
        visited.delete(node.id());
      };

      dfs(startNode, [startNode]);

      // En uzun yolu vurgula
      if (longestPath.length > 0) {
        // Yoldaki düğümleri vurgula
        longestPath.forEach(node => node.addClass('highlight'));

        // Yoldaki kenarları vurgula
        for (let i = 0; i < longestPath.length - 1; i++) {
          const edge = cy.elements(`edge[source = "${longestPath[i].id()}"][target = "${longestPath[i + 1].id()}"], edge[source = "${longestPath[i + 1].id()}"][target = "${longestPath[i].id()}"]`);
          edge.addClass('highlight');
        }

        // Detay panelini güncelle
        selectedNode.value = {
          label: 'En Uzun Yol Bilgisi',
          papers: [],
          collaborationCount: longestPath.length - 1,
          pathInfo: {
            length: longestPath.length - 1,
            nodes: longestPath.map(node => node.data('label'))
          }
        };

        // Yola zoom yap
        cy.animate({
          fit: {
            eles: cy.elements('.highlight'),
            padding: 50
          },
          duration: 500
        });
      }
    };

    const closeDetails = () => {
      selectedNode.value = null;
      cy.elements().removeClass('highlight');
    };

    onMounted(() => {
      initializeCytoscape();
    });

    return {
      cyElement,
      selectedNode,
      searchAuthorId,
      collaborationSearchId,
      averagePapers,
      isDropdownOpen,
      dropdownRef,
      zoomIn,
      zoomOut,
      fit,
      closeDetails,
      showMostCollaborative,
      findLongestPath,
      calculateCollaborations,
      toggleDropdown
    };
  }
};
</script>

<style scoped>
.graph-wrapper {
  position: relative;
  width: 100%;
  height: 100vh;
  background: linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%);
  padding: 20px;
  box-sizing: border-box;
}

.graph-header {
  background: white;
  padding: 15px 25px;
  border-radius: 12px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  margin-bottom: 20px;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.graph-header h2 {
  margin: 0;
  color: #1a237e;
  font-size: 24px;
  font-weight: 600;
}

.graph-stats {
  display: flex;
  gap: 20px;
}

.stat-item {
  background: #f5f5f5;
  padding: 8px 15px;
  border-radius: 8px;
}

.stat-label {
  color: #666;
  margin-right: 8px;
}

.stat-value {
  font-weight: 600;
  color: #1a237e;
}

.graph-container {
  width: 100%;
  height: calc(100vh - 180px);
  background: white;
  border-radius: 12px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  overflow: hidden;
}

.header-controls {
  display: flex;
  align-items: center;
  gap: 20px;
}

.dropdown-container {
  position: relative;
}

.dropdown-btn {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 10px 20px;
  border: none;
  border-radius: 8px;
  background: #1a237e;
  color: white;
  cursor: pointer;
  font-weight: 500;
  transition: all 0.3s ease;
}

.dropdown-btn:hover {
  background: #283593;
}

.dropdown-arrow {
  font-size: 12px;
  transition: transform 0.3s ease;
}

.arrow-up {
  transform: rotate(180deg);
}

.dropdown-content {
  position: absolute;
  top: 100%;
  right: 0;
  margin-top: 8px;
  background: white;
  border-radius: 12px;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.15);
  width: 300px;
  z-index: 1000;
}

.dropdown-section {
  padding: 16px;
  border-bottom: 1px solid #eee;
}

.dropdown-section:last-child {
  border-bottom: none;
}

.dropdown-section h3 {
  margin: 0 0 12px 0;
  color: #1a237e;
  font-size: 16px;
  font-weight: 500;
}

.input-group {
  display: flex;
  gap: 8px;
}

.dropdown-input {
  flex: 1;
  padding: 8px 12px;
  border: 2px solid #1a237e;
  border-radius: 6px;
  font-size: 14px;
  outline: none;
  transition: all 0.3s ease;
}

.dropdown-input:focus {
  border-color: #283593;
  box-shadow: 0 0 0 2px rgba(26, 35, 126, 0.1);
}

.dropdown-action-btn {
  display: flex;
  align-items: center;
  gap: 6px;
  padding: 8px 16px;
  border: none;
  border-radius: 6px;
  background: #1a237e;
  color: white;
  cursor: pointer;
  font-weight: 500;
  transition: all 0.3s ease;
}

.dropdown-action-btn:hover {
  background: #283593;
  transform: translateY(-1px);
}

.dropdown-action-btn.full-width {
  width: 100%;
  justify-content: center;
}

.controls-panel {
  position: fixed;
  bottom: 30px;
  left: 50%;
  transform: translateX(-50%);
  display: flex;
  gap: 15px;
  background: white;
  padding: 12px 20px;
  border-radius: 12px;
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.1);
}

.control-btn {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 10px 20px;
  border: none;
  border-radius: 8px;
  background: #1a237e;
  color: white;
  cursor: pointer;
  font-weight: 500;
  transition: all 0.3s ease;
}

.control-btn:hover {
  background: #283593;
  transform: translateY(-2px);
}

.btn-icon {
  font-size: 18px;
}

.node-details {
  position: fixed;
  top: 20px;
  left: 20px;
  width: 300px;
  background: white;
  border-radius: 12px;
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.1);
  overflow: hidden;
}

.details-header {
  background: #1a237e;
  color: white;
  padding: 15px 20px;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.details-header h3 {
  margin: 0;
  font-size: 18px;
  font-weight: 500;
}

.close-btn {
  background: none;
  border: none;
  color: white;
  font-size: 24px;
  cursor: pointer;
  padding: 0;
  line-height: 1;
}

.details-content {
  padding: 20px;
}

.detail-item {
  margin: 10px 0;
  color: #333;
}

.papers-list {
  margin-top: 15px;
}

.papers-list h4 {
  color: #1a237e;
  margin-bottom: 10px;
}

.papers-list ul {
  list-style: none;
  padding: 0;
  margin: 0;
}

.papers-list li {
  padding: 8px 0;
  border-bottom: 1px solid #eee;
  color: #666;
  font-size: 14px;
}

.more-papers {
  color: #666;
  font-style: italic;
  font-size: 14px;
  margin-top: 10px;
  text-align: center;
  padding: 8px;
  background: #f5f5f5;
  border-radius: 6px;
}

@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(-10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.node-details {
  animation: fadeIn 0.3s ease-out;
}

@media (max-width: 768px) {
  .graph-header {
    flex-direction: column;
    gap: 15px;
  }

  .graph-stats {
    flex-direction: column;
    gap: 10px;
  }

  .controls-panel {
    flex-direction: column;
    bottom: 20px;
    padding: 10px;
  }

  .node-details {
    width: 90%;
    left: 50%;
    transform: translateX(-50%);
    max-height: 80vh;
    overflow-y: auto;
  }
}

.control-group {
  display: flex;
  gap: 10px;
  align-items: center;
}

.author-input {
  padding: 10px;
  border: 2px solid #1a237e;
  border-radius: 8px;
  font-size: 14px;
  width: 100px;
  outline: none;
  transition: all 0.3s ease;
}

.author-input:focus {
  border-color: #283593;
  box-shadow: 0 0 0 2px rgba(26, 35, 126, 0.1);
}

.path-info {
  margin-top: 15px;
}

.path-info h4 {
  color: #1a237e;
  margin-bottom: 10px;
}

.path-info p {
  color: #333;
  margin: 8px 0;
}

.path-nodes {
  margin-top: 10px;
}

.path-nodes h5 {
  color: #666;
  margin-bottom: 8px;
  font-size: 14px;
}

.path-nodes ul {
  list-style: none;
  padding: 0;
  margin: 0;
}

.path-nodes li {
  padding: 6px 0;
  border-bottom: 1px solid #eee;
  color: #333;
  font-size: 14px;
}

.collaboration-info {
  margin-top: 15px;
}

.collaboration-info h4 {
  color: #1a237e;
  margin-bottom: 10px;
}

.collaboration-count {
  background: #e8eaf6;
  padding: 10px;
  border-radius: 6px;
  color: #1a237e;
  font-weight: 500;
  margin: 10px 0;
  text-align: center;
}

.collaborators-list {
  margin-top: 15px;
}

.collaborators-list h5 {
  color: #666;
  margin-bottom: 8px;
  font-size: 14px;
}

.collaborators-list ul {
  list-style: none;
  padding: 0;
  margin: 0;
  max-height: 200px;
  overflow-y: auto;
}

.collaborators-list li {
  padding: 8px 10px;
  border-bottom: 1px solid #eee;
  color: #333;
  font-size: 14px;
  transition: background-color 0.2s;
}

.collaborators-list li:hover {
  background-color: #f5f5f5;
}

/* Scrollbar Styling */
::-webkit-scrollbar {
  width: 8px;
  height: 8px;
}

::-webkit-scrollbar-track {
  background: #f1f1f1;
  border-radius: 4px;
}

::-webkit-scrollbar-thumb {
  background: #888;
  border-radius: 4px;
}

::-webkit-scrollbar-thumb:hover {
  background: #555;
}

</style>

