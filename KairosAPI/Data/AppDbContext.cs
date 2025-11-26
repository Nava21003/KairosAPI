using System;
using System.Collections.Generic;
using KairosAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace KairosAPI.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Actividades> Actividades { get; set; }

    public virtual DbSet<Categoria> Categorias { get; set; }

    public virtual DbSet<Interese> Intereses { get; set; }

    public virtual DbSet<Lugare> Lugares { get; set; }

    // === NUEVO: Tabla de Mensajes ===
    public virtual DbSet<MensajesContacto> MensajesContactos { get; set; }

    public virtual DbSet<Notificacione> Notificaciones { get; set; }

    // === NUEVO: Tabla de FAQ ===
    public virtual DbSet<PreguntasFrecuentes> PreguntasFrecuentes { get; set; }

    public virtual DbSet<Promocione> Promociones { get; set; }

    public virtual DbSet<PuntoInteres> PuntosInteres { get; set; }

    public virtual DbSet<RegistroClic> RegistroClics { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Ruta> Rutas { get; set; }

    public virtual DbSet<RutasLugare> RutasLugares { get; set; }

    public virtual DbSet<SociosAfiliado> SociosAfiliados { get; set; }

    public virtual DbSet<Token> Tokens { get; set; }

    public virtual DbSet<UsoDigital> UsoDigitals { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // NOTA: Recuerda que tener la cadena de conexión aquí no es seguro para producción.
            // Es mejor usar appsettings.json, pero lo dejo como lo enviaste.
            var connectionString = "Server=ISAACGG\\SQLEXPRESS;Database=Kairos;User Id=sa;Password=12345678;TrustServerCertificate=True;";
            optionsBuilder.UseSqlServer(connectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Actividades>(entity =>
        {
            entity.HasKey(e => e.IdActividad).HasName("PK__Activida__327F9BED2ACFE81F");

            entity.Property(e => e.IdActividad).HasColumnName("idActividad");
            entity.Property(e => e.Calificacion).HasColumnName("calificacion");
            entity.Property(e => e.Comentarios)
                .HasMaxLength(255)
                .HasColumnName("comentarios");
            entity.Property(e => e.Distancia)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("distancia");
            entity.Property(e => e.FechaFin)
                .HasColumnType("datetime")
                .HasColumnName("fechaFin");
            entity.Property(e => e.FechaInicio)
                .HasColumnType("datetime")
                .HasColumnName("fechaInicio");
            entity.Property(e => e.IdLugar).HasColumnName("idLugar");
            entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");
            entity.Property(e => e.Pasos).HasColumnName("pasos");

            entity.HasOne(d => d.IdLugarNavigation).WithMany(p => p.Actividades)
                .HasForeignKey(d => d.IdLugar)
                .HasConstraintName("FK__Actividad__idLug__6B24EA82");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Actividades)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK__Actividad__idUsu__6A30C649");
        });

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(e => e.IdCategoria).HasName("PK__Categori__8A3D240C8BD2A978");

            entity.HasIndex(e => e.Nombre, "UQ__Categori__72AFBCC6DE4FF646").IsUnique();

            entity.Property(e => e.IdCategoria).HasColumnName("idCategoria");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Interese>(entity =>
        {
            entity.HasKey(e => e.IdInteres).HasName("PK__Interese__650CDE950F0ABA76");

            entity.Property(e => e.IdInteres).HasColumnName("idInteres");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(255)
                .HasColumnName("descripcion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Lugare>(entity =>
        {
            entity.HasKey(e => e.IdLugar).HasName("PK__Lugares__F7460D5FFEAF67D4");

            entity.Property(e => e.IdLugar).HasColumnName("idLugar");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(500)
                .HasColumnName("descripcion");
            entity.Property(e => e.Direccion)
                .HasMaxLength(255)
                .HasColumnName("direccion");
            entity.Property(e => e.EsPatrocinado).HasDefaultValue(false);
            entity.Property(e => e.Estatus)
                .HasDefaultValue(true)
                .HasColumnName("estatus");
            entity.Property(e => e.Horario)
                .HasMaxLength(100)
                .HasColumnName("horario");
            entity.Property(e => e.IdCategoria).HasColumnName("idCategoria");
            entity.Property(e => e.Imagen)
                .HasMaxLength(255)
                .HasColumnName("imagen");
            entity.Property(e => e.Latitud)
                .HasColumnType("decimal(10, 6)")
                .HasColumnName("latitud");
            entity.Property(e => e.Longitud)
                .HasColumnType("decimal(10, 6)")
                .HasColumnName("longitud");
            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .HasColumnName("nombre");

            entity.HasOne(d => d.IdCategoriaNavigation).WithMany(p => p.Lugares)
                .HasForeignKey(d => d.IdCategoria)
                .HasConstraintName("FK__Lugares__idCateg__4D94879B");
        });

        // === NUEVO: Configuración de MensajesContacto ===
        modelBuilder.Entity<MensajesContacto>(entity =>
        {
            entity.HasKey(e => e.IdMensaje);
            entity.ToTable("MensajesContacto");

            entity.Property(e => e.IdMensaje).HasColumnName("idMensaje");
            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .HasColumnName("nombre");
            entity.Property(e => e.Correo)
                .HasMaxLength(150)
                .HasColumnName("correo");
            entity.Property(e => e.Asunto)
                .HasMaxLength(100)
                .HasColumnName("asunto");
            entity.Property(e => e.Mensaje)
                .HasColumnName("mensaje");
            entity.Property(e => e.FechaEnvio)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaEnvio");
            entity.Property(e => e.Estatus)
                .HasMaxLength(50)
                .HasDefaultValue("Pendiente")
                .HasColumnName("estatus");
        });

        modelBuilder.Entity<Notificacione>(entity =>
        {
            entity.HasKey(e => e.IdNotificacion).HasName("PK__Notifica__AFE1D7E4EE19D0AF");

            entity.Property(e => e.IdNotificacion).HasColumnName("idNotificacion");
            entity.Property(e => e.FechaEnvio)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaEnvio");
            entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");
            entity.Property(e => e.Leido)
                .HasDefaultValue(false)
                .HasColumnName("leido");
            entity.Property(e => e.Mensaje)
                .HasMaxLength(300)
                .HasColumnName("mensaje");
            entity.Property(e => e.Titulo)
                .HasMaxLength(150)
                .HasColumnName("titulo");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Notificaciones)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK__Notificac__idUsu__6E01572D");
        });

        // === NUEVO: Configuración de PreguntasFrecuentes ===
        modelBuilder.Entity<PreguntasFrecuentes>(entity =>
        {
            entity.HasKey(e => e.IdPregunta);
            entity.ToTable("PreguntasFrecuentes");

            entity.Property(e => e.IdPregunta).HasColumnName("idPregunta");
            entity.Property(e => e.Pregunta)
                .HasMaxLength(255)
                .HasColumnName("pregunta");
            entity.Property(e => e.Respuesta)
                .HasColumnName("respuesta");
            entity.Property(e => e.Categoria)
                .HasMaxLength(100)
                .HasDefaultValue("General")
                .HasColumnName("categoria");
            entity.Property(e => e.Orden)
                .HasDefaultValue(0)
                .HasColumnName("orden");
            entity.Property(e => e.Estatus)
                .HasDefaultValue(true)
                .HasColumnName("estatus");
        });

        modelBuilder.Entity<PuntoInteres>(entity =>
        {
            entity.HasKey(e => e.IdPunto).HasName("PK_PuntosInteres");

            entity.ToTable("PuntosInteres");

            entity.Property(e => e.IdPunto).HasColumnName("idPunto");
            entity.Property(e => e.IdLugar).HasColumnName("idLugar");
            entity.Property(e => e.Etiqueta)
                .HasMaxLength(100)
                .HasColumnName("etiqueta");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(255)
                .HasColumnName("descripcion");
            entity.Property(e => e.Prioridad)
                .HasDefaultValue(0)
                .HasColumnName("prioridad");
            entity.Property(e => e.Estatus)
                .HasDefaultValue(true)
                .HasColumnName("estatus");

            entity.HasOne(d => d.IdLugarNavigation).WithMany(p => p.PuntosInteres)
                .HasForeignKey(d => d.IdLugar)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PuntosInteres_Lugares");
        });

        modelBuilder.Entity<Promocione>(entity =>
        {
            entity.HasKey(e => e.IdPromocion).HasName("PK__Promocio__811C0F993146511B");

            entity.Property(e => e.IdPromocion).HasColumnName("idPromocion");
            entity.Property(e => e.Descripcion)
                .HasColumnType("text")
                .HasColumnName("descripcion");
            entity.Property(e => e.Estatus)
                .HasDefaultValue(true)
                .HasColumnName("estatus");
            entity.Property(e => e.FechaFin)
                .HasColumnType("datetime")
                .HasColumnName("fechaFin");
            entity.Property(e => e.FechaInicio)
                .HasColumnType("datetime")
                .HasColumnName("fechaInicio");
            entity.Property(e => e.IdLugar).HasColumnName("idLugar");
            entity.Property(e => e.IdSocio).HasColumnName("idSocio");
            entity.Property(e => e.Titulo)
                .HasMaxLength(150)
                .HasColumnName("titulo");

            entity.HasOne(d => d.IdLugarNavigation).WithMany(p => p.Promociones)
                .HasForeignKey(d => d.IdLugar)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Promocion__idLug__5CD6CB2B");

            entity.HasOne(d => d.IdSocioNavigation).WithMany(p => p.Promociones)
                .HasForeignKey(d => d.IdSocio)
                .HasConstraintName("FK__Promocion__idSoc__5DCAEF64");
        });

        modelBuilder.Entity<RegistroClic>(entity =>
        {
            entity.HasKey(e => e.IdClic).HasName("PK__Registro__8207BDA11BF2B473");

            entity.Property(e => e.IdClic).HasColumnName("idClic");
            entity.Property(e => e.FechaClic)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaClic");
            entity.Property(e => e.IdPromocion).HasColumnName("idPromocion");
            entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");

            entity.HasOne(d => d.IdPromocionNavigation).WithMany(p => p.RegistroClics)
                .HasForeignKey(d => d.IdPromocion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RegistroC__idPro__619B8048");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.RegistroClics)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK__RegistroC__idUsu__628FA481");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.IdRol).HasName("PK__Roles__3C872F7613E405DC");

            entity.HasIndex(e => e.NombreRol, "UQ__Roles__2787B00CFB3315C2").IsUnique();

            entity.Property(e => e.IdRol).HasColumnName("idRol");
            entity.Property(e => e.NombreRol)
                .HasMaxLength(50)
                .HasColumnName("nombreRol");
        });

        modelBuilder.Entity<Ruta>(entity =>
        {
            entity.HasKey(e => e.IdRuta).HasName("PK__Rutas__E584E6F40CD35E1A");

            entity.Property(e => e.IdRuta).HasColumnName("idRuta");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(500)
                .HasColumnName("descripcion");
            entity.Property(e => e.Estatus)
                .HasMaxLength(50)
                .HasDefaultValue("Planificada")
                .HasColumnName("estatus");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaCreacion");
            entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");
            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .HasColumnName("nombre");

            entity.Property(e => e.IdLugarInicio).HasColumnName("idLugarInicio");
            entity.Property(e => e.IdLugarFin).HasColumnName("idLugarFin");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Ruta)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK__Rutas__idUsuario__52593CB8");

            entity.HasOne(d => d.IdLugarInicioNavigation)
                  .WithMany()
                  .HasForeignKey(d => d.IdLugarInicio)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_Rutas_Lugares_Inicio");

            entity.HasOne(d => d.IdLugarFinNavigation)
                  .WithMany()
                  .HasForeignKey(d => d.IdLugarFin)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_Rutas_Lugares_Fin");
        });

        modelBuilder.Entity<RutasLugare>(entity =>
        {
            entity.HasKey(e => new { e.IdRuta, e.IdLugar }).HasName("PK__Rutas_Lu__0AF086217070BDB7");

            entity.ToTable("Rutas_Lugares");

            entity.Property(e => e.IdRuta).HasColumnName("idRuta");
            entity.Property(e => e.IdLugar).HasColumnName("idLugar");
            entity.Property(e => e.Orden).HasColumnName("orden");

            entity.HasOne(d => d.IdLugarNavigation).WithMany(p => p.RutasLugares)
                .HasForeignKey(d => d.IdLugar)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Rutas_Lug__idLug__5812160E");

            entity.HasOne(d => d.IdRutaNavigation).WithMany(p => p.RutasLugares)
                .HasForeignKey(d => d.IdRuta)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Rutas_Lug__idRut__571DF1D5");
        });

        modelBuilder.Entity<SociosAfiliado>(entity =>
        {
            entity.HasKey(e => e.IdSocio).HasName("PK__SociosAf__E19769C170A1B3EC");

            entity.Property(e => e.IdSocio).HasColumnName("idSocio");
            entity.Property(e => e.NombreSocio)
                .HasMaxLength(150)
                .HasColumnName("nombreSocio");
            entity.Property(e => e.TarifaCpc)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("tarifaCPC");
        });

        modelBuilder.Entity<Token>(entity =>
        {
            entity.HasKey(e => e.IdToken).HasName("PK__Tokens__FEFE350DB6D42ABE");

            entity.Property(e => e.IdToken).HasColumnName("idToken");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaRegistro");
            entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");
            entity.Property(e => e.Tipo)
                .HasMaxLength(50)
                .HasColumnName("tipo");
            entity.Property(e => e.Token1)
                .HasMaxLength(500)
                .HasColumnName("token");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Tokens)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK__Tokens__idUsuari__45F365D3");
        });

        modelBuilder.Entity<UsoDigital>(entity =>
        {
            entity.HasKey(e => e.IdUsoDigital).HasName("PK__UsoDigit__E4A8417734A33016");

            entity.ToTable("UsoDigital");

            entity.HasIndex(e => e.FechaRegistro, "UQ__UsoDigit__D7F4BC9983B16C00").IsUnique();

            entity.Property(e => e.IdUsoDigital).HasColumnName("idUsoDigital");
            entity.Property(e => e.FechaRegistro).HasColumnName("fechaRegistro");
            entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");
            entity.Property(e => e.TiempoDigitalMinutos).HasColumnName("tiempoDigitalMinutos");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.UsoDigitals)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK__UsoDigita__idUsu__6754599E");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK__Usuarios__645723A6B9C4D14F");

            entity.HasIndex(e => e.Correo, "UQ__Usuarios__2A586E0B2CC327DD").IsUnique();

            entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");
            entity.Property(e => e.Apellido)
                .HasMaxLength(100)
                .HasColumnName("apellido");
            entity.Property(e => e.Contrasena)
                .HasMaxLength(255)
                .HasColumnName("contrasena");
            entity.Property(e => e.Correo)
                .HasMaxLength(150)
                .HasColumnName("correo");
            entity.Property(e => e.Estatus)
                .HasDefaultValue(true)
                .HasColumnName("estatus");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaRegistro");
            entity.Property(e => e.FotoPerfil)
                .HasMaxLength(255)
                .HasColumnName("fotoPerfil");
            entity.Property(e => e.IdRol).HasColumnName("idRol");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdRol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Usuarios__idRol__3B75D760");

            entity.HasMany(d => d.IdInteres).WithMany(p => p.IdUsuarios)
                .UsingEntity<Dictionary<string, object>>(
                    "UsuariosInterese",
                    r => r.HasOne<Interese>().WithMany()
                        .HasForeignKey("IdInteres")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Usuarios___idInt__4316F928"),
                    l => l.HasOne<Usuario>().WithMany()
                        .HasForeignKey("IdUsuario")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Usuarios___idUsu__4222D4EF"),
                    j =>
                    {
                        j.HasKey("IdUsuario", "IdInteres").HasName("PK__Usuarios__2207EE4FE5A700C6");
                        j.ToTable("Usuarios_Intereses");
                        j.IndexerProperty<int>("IdUsuario").HasColumnName("idUsuario");
                        j.IndexerProperty<int>("IdInteres").HasColumnName("idInteres");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}