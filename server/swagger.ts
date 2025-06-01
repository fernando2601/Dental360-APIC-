import swaggerJSDoc from 'swagger-jsdoc';
import swaggerUi from 'swagger-ui-express';
import { Express } from 'express';

const options = {
  definition: {
    openapi: '3.0.0',
    info: {
      title: 'DentalSpa API',
      version: '1.0.0',
      description: 'API completa para sistema de gestão de clínica odontológica e harmonização facial',
      contact: {
        name: 'DentalSpa Team',
        email: 'contato@dentalspa.com'
      }
    },
    servers: [
      {
        url: 'http://localhost:5000',
        description: 'Servidor de Desenvolvimento'
      }
    ],
    components: {
      securitySchemes: {
        bearerAuth: {
          type: 'http',
          scheme: 'bearer',
          bearerFormat: 'JWT'
        }
      },
      schemas: {
        User: {
          type: 'object',
          properties: {
            id: { type: 'number' },
            username: { type: 'string' },
            fullName: { type: 'string' },
            email: { type: 'string' },
            role: { type: 'string', enum: ['admin', 'funcionario', 'dentista'] },
            phone: { type: 'string', nullable: true },
            createdAt: { type: 'string', format: 'date-time' }
          }
        },
        Client: {
          type: 'object',
          properties: {
            id: { type: 'number' },
            fullName: { type: 'string' },
            email: { type: 'string' },
            phone: { type: 'string' },
            address: { type: 'string', nullable: true },
            birthday: { type: 'string', format: 'date', nullable: true },
            notes: { type: 'string', nullable: true },
            createdAt: { type: 'string', format: 'date-time' }
          }
        },
        Service: {
          type: 'object',
          properties: {
            id: { type: 'number' },
            name: { type: 'string' },
            category: { type: 'string' },
            description: { type: 'string' },
            duration: { type: 'number' },
            price: { type: 'string' },
            active: { type: 'boolean' }
          }
        },
        Appointment: {
          type: 'object',
          properties: {
            id: { type: 'number' },
            clientId: { type: 'number' },
            staffId: { type: 'number' },
            serviceId: { type: 'number' },
            startTime: { type: 'string', format: 'date-time' },
            endTime: { type: 'string', format: 'date-time' },
            status: { type: 'string', enum: ['agendado', 'confirmado', 'em_andamento', 'concluido', 'cancelado'] },
            notes: { type: 'string', nullable: true },
            createdAt: { type: 'string', format: 'date-time' }
          }
        },
        Staff: {
          type: 'object',
          properties: {
            id: { type: 'number' },
            userId: { type: 'number' },
            specialization: { type: 'string' },
            bio: { type: 'string', nullable: true },
            available: { type: 'boolean' }
          }
        },
        InventoryItem: {
          type: 'object',
          properties: {
            id: { type: 'number' },
            name: { type: 'string' },
            category: { type: 'string' },
            description: { type: 'string', nullable: true },
            price: { type: 'string', nullable: true },
            quantity: { type: 'number' },
            unit: { type: 'string' },
            threshold: { type: 'number', nullable: true },
            lastRestocked: { type: 'string', format: 'date-time', nullable: true }
          }
        },
        FinancialTransaction: {
          type: 'object',
          properties: {
            id: { type: 'number' },
            date: { type: 'string', format: 'date-time' },
            type: { type: 'string', enum: ['receita', 'despesa'] },
            category: { type: 'string' },
            description: { type: 'string', nullable: true },
            clientId: { type: 'number', nullable: true },
            appointmentId: { type: 'number', nullable: true },
            amount: { type: 'string' },
            paymentMethod: { type: 'string', nullable: true }
          }
        },
        Package: {
          type: 'object',
          properties: {
            id: { type: 'number' },
            name: { type: 'string' },
            description: { type: 'string' },
            services: { type: 'array', items: { type: 'string' } },
            originalPrice: { type: 'string' },
            packagePrice: { type: 'string' },
            discount: { type: 'string' },
            validityDays: { type: 'number' },
            active: { type: 'boolean' },
            createdAt: { type: 'string', format: 'date-time' }
          }
        },
        BeforeAfterCase: {
          type: 'object',
          properties: {
            id: { type: 'number' },
            patientId: { type: 'number' },
            title: { type: 'string' },
            description: { type: 'string' },
            procedure: { type: 'string' },
            beforeImages: { type: 'array', items: { type: 'string' } },
            afterImages: { type: 'array', items: { type: 'string' } },
            datePerformed: { type: 'string', format: 'date-time' },
            isPublic: { type: 'boolean' },
            patientConsent: { type: 'boolean' },
            createdAt: { type: 'string', format: 'date-time' }
          }
        },
        Error: {
          type: 'object',
          properties: {
            message: { type: 'string' },
            error: { type: 'string' }
          }
        }
      }
    },
    security: [
      {
        bearerAuth: []
      }
    ]
  },
  apis: ['./server/routes.ts', './server/auth.ts']
};

const specs = swaggerJSDoc(options);

export function setupSwagger(app: Express): void {
  app.use('/api-docs', swaggerUi.serve, swaggerUi.setup(specs, {
    explorer: true,
    customCss: '.swagger-ui .topbar { display: none }',
    customSiteTitle: 'DentalSpa API Documentation'
  }));
  
  console.log('📚 Swagger UI disponível em: http://localhost:5000/api-docs');
}

export { specs };