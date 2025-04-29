# Excel Graph Visualizer

A powerful fullstack application that transforms Excel data into interactive network visualizations. This tool combines a robust .NET backend with a responsive Vue.js frontend to provide seamless data processing and advanced graph visualization capabilities.


## ✨ Features

- **Excel Data Processing**: Parse and extract structured data from Excel files
- **Graph Transformation**: Convert tabular data into graph-based structures
- **Interactive Visualization**: Explore data relationships through dynamic network graphs
- **Advanced Filtering**: Apply detailed filters to focus on specific data subsets
- **Single-Page Application**: Smooth user experience with no page reloads
- **Responsive Design**: Optimized for both desktop and mobile devices
- **Data Export**: Save visualizations and processed data in various formats

## 🖥️ Technology Stack

### Backend (.NET)
- **ASP.NET Core Web API**
- **EPPlus/ExcelDataReader**: For Excel file parsing
- **Entity Framework Core**: For data persistence (if applicable)
- **Dependency Injection**: For maintainable service architecture
- **Swagger/OpenAPI**: For API documentation

### Frontend (Vue.js)
- **Vue 3**: With Composition API
- **Vis.js**: For interactive network visualizations
- **Vuex/Pinia**: For state management
- **Vue Router**: For SPA navigation
- **Tailwind CSS/Bootstrap**: For responsive UI components
- **Axios**: For API communication

## 📊 How It Works

1. **Upload Excel File**: Users upload an Excel file through the intuitive interface
2. **Backend Processing**:
   - The .NET backend parses the Excel file into structured data
   - GraphPower library processes this data into a network graph structure
   - The processed graph is optimized and returned to the frontend
3. **Visualization**: 
   - Vis.js renders the graph as an interactive network visualization
   - Users can explore relationships, zoom, pan, and interact with nodes
4. **Data Analysis**:
   - Apply filters to focus on specific data subsets
   - View detailed information about selected nodes and connections
   - Discover patterns and insights within complex data structures

## 🛠️ Architecture

```
Excel-Graph-Visualizer/
├── Backend/
│   ├── ExcelGraphVisualizer.API/          # Web API project
│   │   ├── Controllers/                   # API endpoints
│   │   ├── Services/                      # Business logic
│   │   │   ├── ExcelParserService.cs      # Excel file processing
│   │   │   ├── GraphProcessorService.cs   # Graph transformation
│   │   │   └── VisualizationService.cs    # Visualization data prep
│   │   ├── Models/                        # Data models
│   │   └── Program.cs                     # App configuration
│   ├── ExcelGraphVisualizer.Core/         # Core business logic
│   └── ExcelGraphVisualizer.Tests/        # Unit tests
│
├── Frontend/
│   ├── public/                            # Static assets
│   ├── src/
│   │   ├── assets/                        # Images and global styles
│   │   ├── components/                    # Vue components
│   │   │   ├── FileUpload.vue             # Excel file uploader
│   │   │   ├── GraphVisualization.vue     # Vis.js implementation
│   │   │   ├── FilterPanel.vue            # Data filtering options
│   │   │   └── DetailView.vue             # Node/edge details
│   │   ├── services/                      # API services
│   │   ├── store/                         # Vuex/Pinia store
│   │   ├── views/                         # Page components
│   │   ├── App.vue                        # Root component
│   │   └── main.js                        # Entry point
│   ├── package.json                       # Dependencies
│   └── vite.config.js                     # Build configuration
│
└── README.md                              # Project documentation
```

## 🚀 Getting Started

### Prerequisites

- .NET 6.0 SDK or later
- Node.js 14.x or later
- npm or yarn

### Backend Setup

1. Clone the repository
```bash
git clone https://github.com/yourusername/excel-graph-visualizer.git
cd excel-graph-visualizer
```

2. Navigate to the backend directory
```bash
cd Backend/ExcelGraphVisualizer.API
```

3. Build and run the application
```bash
dotnet build
dotnet run
```

The API will be available at `https://localhost:5001`

### Frontend Setup

1. Navigate to the frontend directory
```bash
cd ../../Frontend
```

2. Install dependencies
```bash
npm install
# or
yarn install
```

3. Start the development server
```bash
npm run dev
# or
yarn dev
```

The application will be available at `http://localhost:3000`

## 🔍 API Endpoints

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/api/excel/upload` | POST | Upload and process Excel file |
| `/api/graph/generate` | POST | Generate graph from processed data |
| `/api/visualization/config` | GET | Get visualization configuration options |
| `/api/filters` | GET | Get available filter options |

## 💻 Usage Examples

### Uploading an Excel File

```javascript
// Vue component code
async uploadFile() {
  const formData = new FormData();
  formData.append('file', this.selectedFile);
  
  try {
    const response = await axios.post('/api/excel/upload', formData, {
      headers: {
        'Content-Type': 'multipart/form-data'
      }
    });
    
    this.processedData = response.data;
    this.generateGraph();
  } catch (error) {
    console.error('Error uploading file:', error);
  }
}
```

### Visualizing Graph Data

```javascript
// Vue component using vis.js
createVisualization(graphData) {
  const container = document.getElementById('visualization');
  
  const options = {
    nodes: {
      shape: 'dot',
      scaling: {
        min: 10,
        max: 30
      }
    },
    edges: {
      smooth: {
        type: 'continuous'
      }
    },
    physics: {
      stabilization: true,
      barnesHut: {
        gravitationalConstant: -80000,
        springConstant: 0.001,
        springLength: 200
      }
    },
    interaction: {
      tooltipDelay: 200,
      hideEdgesOnDrag: true
    }
  };
  
  this.network = new vis.Network(container, graphData, options);
  
  // Add event listeners
  this.network.on('selectNode', this.handleNodeSelection);
}
```

## 🔧 Advanced Features

### Filtering Options

The application provides several filtering mechanisms:

- **Node Types**: Filter by different types of nodes in your data
- **Connection Strength**: Filter connections based on relationship strength
- **Data Attributes**: Filter by specific data properties or values
- **Hierarchy Levels**: Show only specific levels of hierarchy
- **Date Ranges**: Filter data within specific time periods
- **Custom Expressions**: Create complex filter expressions

### Visualization Customization

- **Layout Algorithms**: Choose different layout algorithms for optimal visualization
- **Color Schemes**: Apply custom color schemes based on data attributes
- **Node Sizing**: Size nodes based on importance metrics
- **Edge Styling**: Customize edge appearance based on relationship types
- **Clustering**: Automatically group related nodes
- **Focus Areas**: Highlight important sections of the graph

## 📊 Example Use Cases

- **Organizational Hierarchies**: Visualize company structures from HR Excel data
- **Financial Networks**: Analyze transaction relationships between accounts
- **Project Dependencies**: Map interdependencies between project tasks
- **Supply Chain Visualization**: Understand complex supply networks
- **Data Relationships**: Explore connections between related data entities



## 📝 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🙏 Acknowledgements

- [Vis.js](https://visjs.org/) for the powerful visualization library
- [ExcelDataReader](https://github.com/ExcelDataReader/ExcelDataReader) for Excel parsing
- [Vue.js](https://vuejs.org/) for the frontend framework
- [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/) for the backend API

