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
    public virtual DbSet<MensajesContacto> MensajesContactos { get; set; }
    public virtual DbSet<Notificacione> Notificaciones { get; set; }
    public virtual DbSet<PreguntasFrecuentes> PreguntasFrecuentes { get; set; }
    public virtual DbSet<Promocione> Promociones { get; set; }
    public virtual DbSet<PuntoInteres> PuntosInteres { get; set; }
    public virtual DbSet<RegistroClic> RegistroClics { get; set; }
    public virtual DbSet<Resenas> Resenas { get; set; }
    public virtual DbSet<Role> Roles { get; set; }
    public virtual DbSet<Ruta> Rutas { get; set; }
    public virtual DbSet<RutasLugare> RutasLugares { get; set; }
    public virtual DbSet<SociosAfiliado> SociosAfiliados { get; set; }
    public virtual DbSet<Token> Tokens { get; set; }
    public virtual DbSet<UsoDigital> UsoDigitals { get; set; }
    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Resenas>(entity =>
        {
            entity.HasKey(e => e.IdResena);
            entity.ToTable("Resenas");

            entity.Property(e => e.IdResena).HasColumnName("idResena");

            entity.Property(e => e.UsuarioNombre)
                .HasMaxLength(100)
                .HasColumnName("usuarioNombre");

            entity.Property(e => e.Rol)
                .HasMaxLength(50)
                .HasDefaultValue("Explorador")
                .HasColumnName("rol");

            entity.Property(e => e.Comentario)
                .HasColumnName("comentario");

            entity.Property(e => e.Estrellas)
                .HasColumnName("estrellas");

            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("GETDATE()")
                .HasColumnType("datetime")
                .HasColumnName("fechaRegistro");

            entity.Property(e => e.Estatus)
                .HasDefaultValue(true)
                .HasColumnName("estatus");
        });


        modelBuilder.Entity<Actividades>(entity =>
        {
            entity.HasKey(e => e.IdActividad).HasName("PK_Actividades");
            entity.Property(e => e.IdActividad).HasColumnName("idActividad");
            entity.Property(e => e.Calificacion).HasColumnName("calificacion");
            entity.Property(e => e.Comentarios).HasMaxLength(255).HasColumnName("comentarios");
            entity.Property(e => e.Distancia).HasColumnType("decimal(10, 2)").HasColumnName("distancia");
            entity.Property(e => e.FechaFin).HasColumnType("datetime").HasColumnName("fechaFin");
            entity.Property(e => e.FechaInicio).HasColumnType("datetime").HasColumnName("fechaInicio");
            entity.Property(e => e.IdLugar).HasColumnName("idLugar");
            entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");
            entity.Property(e => e.Pasos).HasColumnName("pasos");

            entity.HasOne(d => d.IdLugarNavigation).WithMany(p => p.Actividades)
                .HasForeignKey(d => d.IdLugar).HasConstraintName("FK_Actividades_Lugares");
            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Actividades)
                .HasForeignKey(d => d.IdUsuario).HasConstraintName("FK_Actividades_Usuarios");
        });

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(e => e.IdCategoria).HasName("PK_Categorias");
            entity.HasIndex(e => e.Nombre, "UQ_Categorias_Nombre").IsUnique();
            entity.Property(e => e.IdCategoria).HasColumnName("idCategoria");
            entity.Property(e => e.Nombre).HasMaxLength(100).HasColumnName("nombre");
            entity.Property(e => e.Descripcion).HasMaxLength(255).HasColumnName("descripcion");
            entity.Property(e => e.Estatus).HasDefaultValue(true).HasColumnName("estatus");
        });

        modelBuilder.Entity<Interese>(entity =>
        {
            entity.HasKey(e => e.IdInteres).HasName("PK_Intereses");
            entity.Property(e => e.IdInteres).HasColumnName("idInteres");
            entity.Property(e => e.Descripcion).HasMaxLength(255).HasColumnName("descripcion");
            entity.Property(e => e.Nombre).HasMaxLength(100).HasColumnName("nombre");
        });

        modelBuilder.Entity<Lugare>(entity =>
        {
            entity.HasKey(e => e.IdLugar).HasName("PK_Lugares");
            entity.Property(e => e.IdLugar).HasColumnName("idLugar");
            entity.Property(e => e.Descripcion).HasMaxLength(500).HasColumnName("descripcion");
            entity.Property(e => e.Direccion).HasMaxLength(255).HasColumnName("direccion");
            entity.Property(e => e.EsPatrocinado).HasDefaultValue(false).HasColumnName("EsPatrocinado");
            entity.Property(e => e.Estatus).HasDefaultValue(true).HasColumnName("estatus");
            entity.Property(e => e.Horario).HasMaxLength(100).HasColumnName("horario");
            entity.Property(e => e.IdCategoria).HasColumnName("idCategoria");
            entity.Property(e => e.Imagen).IsUnicode(false).HasColumnName("imagen");
            entity.Property(e => e.Latitud).HasColumnType("decimal(10, 6)").HasColumnName("latitud");
            entity.Property(e => e.Longitud).HasColumnType("decimal(10, 6)").HasColumnName("longitud");
            entity.Property(e => e.Nombre).HasMaxLength(150).HasColumnName("nombre");

            entity.HasOne(d => d.IdCategoriaNavigation).WithMany(p => p.Lugares)
                .HasForeignKey(d => d.IdCategoria).HasConstraintName("FK_Lugares_Categorias");
        });

        modelBuilder.Entity<MensajesContacto>(entity =>
        {
            entity.HasKey(e => e.IdMensaje);
            entity.ToTable("MensajesContacto");
            entity.Property(e => e.IdMensaje).HasColumnName("idMensaje");
            entity.Property(e => e.Nombre).HasMaxLength(150).HasColumnName("nombre");
            entity.Property(e => e.Correo).HasMaxLength(150).HasColumnName("correo");
            entity.Property(e => e.Asunto).HasMaxLength(100).HasColumnName("asunto");
            entity.Property(e => e.Mensaje).HasColumnName("mensaje");
            entity.Property(e => e.FechaEnvio).HasDefaultValueSql("GETDATE()").HasColumnType("datetime").HasColumnName("fechaEnvio");
            entity.Property(e => e.Estatus).HasMaxLength(50).HasDefaultValue("Pendiente").HasColumnName("estatus");
        });

        modelBuilder.Entity<Notificacione>(entity =>
        {
            entity.HasKey(e => e.IdNotificacion).HasName("PK_Notificaciones");
            entity.Property(e => e.IdNotificacion).HasColumnName("idNotificacion");
            entity.Property(e => e.FechaEnvio).HasDefaultValueSql("GETDATE()").HasColumnType("datetime").HasColumnName("fechaEnvio");
            entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");
            entity.Property(e => e.Leido).HasDefaultValue(false).HasColumnName("leido");
            entity.Property(e => e.Mensaje).HasMaxLength(300).HasColumnName("mensaje");
            entity.Property(e => e.Titulo).HasMaxLength(150).HasColumnName("titulo");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Notificaciones)
                .HasForeignKey(d => d.IdUsuario).HasConstraintName("FK_Notificaciones_Usuarios");
        });

        modelBuilder.Entity<PreguntasFrecuentes>(entity =>
        {
            entity.HasKey(e => e.IdPregunta);
            entity.ToTable("PreguntasFrecuentes");
            entity.Property(e => e.IdPregunta).HasColumnName("idPregunta");
            entity.Property(e => e.Pregunta).HasMaxLength(255).HasColumnName("pregunta");
            entity.Property(e => e.Respuesta).HasColumnName("respuesta");
            entity.Property(e => e.Categoria).HasMaxLength(100).HasDefaultValue("General").HasColumnName("categoria");
            entity.Property(e => e.Orden).HasDefaultValue(0).HasColumnName("orden");
            entity.Property(e => e.Estatus).HasDefaultValue(true).HasColumnName("estatus");
        });

        modelBuilder.Entity<PuntoInteres>(entity =>
        {
            entity.HasKey(e => e.IdPunto).HasName("PK_PuntosInteres");
            entity.ToTable("PuntosInteres");
            entity.Property(e => e.IdPunto).HasColumnName("idPunto");
            entity.Property(e => e.IdLugar).HasColumnName("idLugar");
            entity.Property(e => e.Etiqueta).HasMaxLength(100).HasColumnName("etiqueta");
            entity.Property(e => e.Descripcion).HasMaxLength(255).HasColumnName("descripcion");
            entity.Property(e => e.Prioridad).HasDefaultValue(0).HasColumnName("prioridad");
            entity.Property(e => e.Estatus).HasDefaultValue(true).HasColumnName("estatus");

            entity.HasOne(d => d.IdLugarNavigation).WithMany(p => p.PuntosInteres)
                .HasForeignKey(d => d.IdLugar).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_PuntosInteres_Lugares");
        });

        modelBuilder.Entity<Promocione>(entity =>
        {
            entity.HasKey(e => e.IdPromocion).HasName("PK_Promociones");
            entity.Property(e => e.IdPromocion).HasColumnName("idPromocion");
            entity.Property(e => e.Descripcion).HasColumnType("text").HasColumnName("descripcion");
            entity.Property(e => e.Estatus).HasDefaultValue(true).HasColumnName("estatus");
            entity.Property(e => e.FechaFin).HasColumnType("datetime").HasColumnName("fechaFin");
            entity.Property(e => e.FechaInicio).HasColumnType("datetime").HasColumnName("fechaInicio");
            entity.Property(e => e.IdLugar).HasColumnName("idLugar");
            entity.Property(e => e.IdSocio).HasColumnName("idSocio");
            entity.Property(e => e.Titulo).HasMaxLength(150).HasColumnName("titulo");
            entity.Property(e => e.PuntosRequeridos).HasDefaultValue(0).HasColumnName("puntosRequeridos");
            entity.HasOne(d => d.IdLugarNavigation).WithMany(p => p.Promociones)
                .HasForeignKey(d => d.IdLugar).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_Promociones_Lugares");
            entity.HasOne(d => d.IdSocioNavigation).WithMany(p => p.Promociones)
                .HasForeignKey(d => d.IdSocio).HasConstraintName("FK_Promociones_Socios");
        });

        modelBuilder.Entity<RegistroClic>(entity =>
        {
            entity.HasKey(e => e.IdClic).HasName("PK_RegistroClics");
            entity.Property(e => e.IdClic).HasColumnName("idClic");
            entity.Property(e => e.FechaClic).HasDefaultValueSql("GETDATE()").HasColumnType("datetime").HasColumnName("fechaClic");
            entity.Property(e => e.IdPromocion).HasColumnName("idPromocion");
            entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");

            entity.HasOne(d => d.IdPromocionNavigation).WithMany(p => p.RegistroClics)
                .HasForeignKey(d => d.IdPromocion).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_RegistroClics_Promociones");
            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.RegistroClics)
                .HasForeignKey(d => d.IdUsuario).HasConstraintName("FK_RegistroClics_Usuarios");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.IdRol).HasName("PK_Roles");
            entity.HasIndex(e => e.NombreRol, "UQ_Roles_Nombre").IsUnique();
            entity.Property(e => e.IdRol).HasColumnName("idRol");
            entity.Property(e => e.NombreRol).HasMaxLength(50).HasColumnName("nombreRol");
        });

        modelBuilder.Entity<Ruta>(entity =>
        {
            entity.HasKey(e => e.IdRuta).HasName("PK_Rutas");
            entity.Property(e => e.IdRuta).HasColumnName("idRuta");
            entity.Property(e => e.Descripcion).HasMaxLength(500).HasColumnName("descripcion");
            entity.Property(e => e.Estatus).HasMaxLength(50).HasDefaultValue("Planificada").HasColumnName("estatus");
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("GETDATE()").HasColumnType("datetime").HasColumnName("fechaCreacion");
            entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");
            entity.Property(e => e.Nombre).HasMaxLength(150).HasColumnName("nombre");
            entity.Property(e => e.IdLugarInicio).HasColumnName("idLugarInicio");
            entity.Property(e => e.IdLugarFin).HasColumnName("idLugarFin");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Ruta)
                .HasForeignKey(d => d.IdUsuario).HasConstraintName("FK_Rutas_Usuarios");
            entity.HasOne(d => d.IdLugarInicioNavigation).WithMany().HasForeignKey(d => d.IdLugarInicio)
                .OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_Rutas_Lugares_Inicio");
            entity.HasOne(d => d.IdLugarFinNavigation).WithMany().HasForeignKey(d => d.IdLugarFin)
                .OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_Rutas_Lugares_Fin");
        });

        modelBuilder.Entity<RutasLugare>(entity =>
        {
            entity.HasKey(e => new { e.IdRuta, e.IdLugar }).HasName("PK_Rutas_Lugares");
            entity.ToTable("Rutas_Lugares");
            entity.Property(e => e.IdRuta).HasColumnName("idRuta");
            entity.Property(e => e.IdLugar).HasColumnName("idLugar");
            entity.Property(e => e.Orden).HasColumnName("orden");

            entity.HasOne(d => d.IdLugarNavigation).WithMany(p => p.RutasLugares)
                .HasForeignKey(d => d.IdLugar).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_RutasLugares_Lugares");
            entity.HasOne(d => d.IdRutaNavigation).WithMany(p => p.RutasLugares)
                .HasForeignKey(d => d.IdRuta).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_RutasLugares_Rutas");
        });

        modelBuilder.Entity<SociosAfiliado>(entity =>
        {
            entity.HasKey(e => e.IdSocio).HasName("PK_SociosAfiliados");
            entity.Property(e => e.IdSocio).HasColumnName("idSocio");
            entity.Property(e => e.NombreSocio).HasMaxLength(150).HasColumnName("nombreSocio");
            entity.Property(e => e.TarifaCpc).HasColumnType("decimal(5, 2)").HasColumnName("tarifaCPC");
            entity.Property(e => e.Estatus).HasDefaultValue(true).HasColumnName("estatus");
        });

        modelBuilder.Entity<Token>(entity =>
        {
            entity.HasKey(e => e.IdToken).HasName("PK_Tokens");
            entity.Property(e => e.IdToken).HasColumnName("idToken");
            entity.Property(e => e.FechaRegistro).HasDefaultValueSql("GETDATE()").HasColumnType("datetime").HasColumnName("fechaRegistro");
            entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");
            entity.Property(e => e.Tipo).HasMaxLength(50).HasColumnName("tipo");
            entity.Property(e => e.Token1).HasMaxLength(500).HasColumnName("token");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Tokens)
                .HasForeignKey(d => d.IdUsuario).HasConstraintName("FK_Tokens_Usuarios");
        });

        modelBuilder.Entity<UsoDigital>(entity =>
        {
            entity.HasKey(e => e.IdUsoDigital).HasName("PK_UsoDigital");
            entity.ToTable("UsoDigital");
            entity.HasIndex(e => new { e.IdUsuario, e.FechaRegistro }, "UQ_UsoDigital_Usuario_Fecha").IsUnique();
            entity.Property(e => e.IdUsoDigital).HasColumnName("idUsoDigital");
            entity.Property(e => e.FechaRegistro).HasColumnType("date").HasColumnName("fechaRegistro");
            entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");
            entity.Property(e => e.TiempoDigitalMinutos).HasColumnName("tiempoDigitalMinutos");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.UsoDigitals)
                .HasForeignKey(d => d.IdUsuario).HasConstraintName("FK_UsoDigital_Usuarios");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK_Usuarios");
            entity.HasIndex(e => e.Correo, "UQ_Usuarios_Correo").IsUnique();
            entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");
            entity.Property(e => e.Apellido).HasMaxLength(100).HasColumnName("apellido");
            entity.Property(e => e.Contrasena).HasMaxLength(255).HasColumnName("contrasena");
            entity.Property(e => e.Correo).HasMaxLength(150).HasColumnName("correo");
            entity.Property(e => e.Estatus).HasDefaultValue(true).HasColumnName("estatus");
            entity.Property(e => e.FechaRegistro).HasDefaultValueSql("GETDATE()").HasColumnType("datetime").HasColumnName("fechaRegistro");
            entity.Property(e => e.FotoPerfil).IsUnicode(false).HasColumnName("fotoPerfil");
            entity.Property(e => e.IdRol).HasColumnName("idRol");
            entity.Property(e => e.Nombre).HasMaxLength(100).HasColumnName("nombre");
            entity.Property(e => e.PuntosAcumulados).HasDefaultValue(0).HasColumnName("puntosAcumulados");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdRol).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_Usuarios_Roles");

            entity.HasMany(d => d.IdInteres).WithMany(p => p.IdUsuarios)
                .UsingEntity<Dictionary<string, object>>(
                    "UsuariosInterese",
                    r => r.HasOne<Interese>().WithMany().HasForeignKey("IdInteres").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_UsuariosIntereses_Intereses"),
                    l => l.HasOne<Usuario>().WithMany().HasForeignKey("IdUsuario").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_UsuariosIntereses_Usuarios"),
                    j =>
                    {
                        j.HasKey("IdUsuario", "IdInteres").HasName("PK_Usuarios_Intereses");
                        j.ToTable("Usuarios_Intereses");
                        j.IndexerProperty<int>("IdUsuario").HasColumnName("idUsuario");
                        j.IndexerProperty<int>("IdInteres").HasColumnName("idInteres");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}