// mockData.js
export const mockGraphData = {
    authors: [
      {
        id: 1,
        name: "John Smith",
        papers: [
          "Machine Learning Applications in Healthcare",
          "Deep Learning for Medical Imaging",
          "AI in Clinical Decision Support"
        ],
        collaborations: {
          "2": 2,
          "3": 1,
          "4": 1
        },
        totalPapers: 3,
        isHighPerforming: true,
        isLowPerforming: false
      },
      {
        id: 2,
        name: "Emily Johnson",
        papers: [
          "Machine Learning Applications in Healthcare",
          "Neural Networks in Medicine",
          "Deep Learning for Medical Imaging"
        ],
        collaborations: {
          "1": 2,
          "3": 2,
          "5": 1
        },
        totalPapers: 3,
        isHighPerforming: true,
        isLowPerforming: false
      },
      {
        id: 3,
        name: "Michael Brown",
        papers: [
          "Machine Learning Applications in Healthcare",
          "Neural Networks in Medicine"
        ],
        collaborations: {
          "1": 1,
          "2": 2
        },
        totalPapers: 2,
        isHighPerforming: false,
        isLowPerforming: true
      },
      {
        id: 4,
        name: "Sarah Davis",
        papers: [
          "AI in Clinical Decision Support",
          "Healthcare Analytics"
        ],
        collaborations: {
          "1": 1,
          "5": 1
        },
        totalPapers: 2,
        isHighPerforming: false,
        isLowPerforming: true
      },
      {
        id: 5,
        name: "David Wilson",
        papers: [
          "Neural Networks in Medicine",
          "Healthcare Analytics"
        ],
        collaborations: {
          "2": 1,
          "4": 1
        },
        totalPapers: 2,
        isHighPerforming: false,
        isLowPerforming: true
      }
    ],
    averagePaperCount: 2.4,
    topCollaborators: [
      {
        author: {
          id: 2,
          name: "Emily Johnson",
          papers: [
            "Machine Learning Applications in Healthcare",
            "Neural Networks in Medicine",
            "Deep Learning for Medical Imaging"
          ],
          collaborations: {
            "1": 2,
            "3": 2,
            "5": 1
          },
          totalPapers: 3,
          isHighPerforming: true,
          isLowPerforming: false
        },
        collaborationCount: 3
      },
      {
        author: {
          id: 1,
          name: "John Smith",
          papers: [
            "Machine Learning Applications in Healthcare",
            "Deep Learning for Medical Imaging",
            "AI in Clinical Decision Support"
          ],
          collaborations: {
            "2": 2,
            "3": 1,
            "4": 1
          },
          totalPapers: 3,
          isHighPerforming: true,
          isLowPerforming: false
        },
        collaborationCount: 3
      }
    ]
  };
